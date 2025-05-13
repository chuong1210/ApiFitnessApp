using Application.Features.ScheduledMeals.Commands.UpdateScheduledMealStatus;
using Domain.Enums;
using Domain.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UpdateScheduledMealStatusCommandValidator : AbstractValidator<UpdateScheduledMealStatusCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduledMealStatusCommandValidator(IUnitOfWork unitOfWork = null)
    {
        _unitOfWork = unitOfWork;
        RuleFor(v => v.ScheduleId).GreaterThan(0);
        RuleFor(v => v.ScheduleId)
 .GreaterThan(0).WithMessage("Invalid Schedule ID.")
 .MustAsync(async (scheduleId, cancellationToken) =>
 {
     var scheduledMeal = await _unitOfWork.ScheduledMeals.GetByIdAsync(scheduleId, cancellationToken);
     return scheduledMeal != null;
 })
 .WithMessage(cmd => $"Scheduled meal with ID {cmd.ScheduleId} not found.")
 .When(v => v.ScheduleId > 0);
        RuleFor(v => v.NewStatus).IsInEnum().WithMessage("Invalid status provided.");
        // Không cho phép chuyển về Planned nếu đã là Eaten/Skipped
        //RuleFor(v => v.NewStatus)
        //   .NotEqual(Domain.Enums.ScheduleStatus.Planned)
        //   .WhenAsync(async (cmd, ct) => { /* logic kiểm tra trạng thái hiện tại của schedule */ return true; })
        //   .WithMessage("Cannot revert to Planned status from Eaten or Skipped.");
        //_unitOfWork = unitOfWork;

        RuleFor(command => command) // Validate toàn bộ command object
          .MustAsync(BeValidStatusTransition)
          .WithMessage(cmd => $"Cannot change status to '{cmd.NewStatus}' from the current status.")
          // Chỉ áp dụng rule này khi NewStatus là Planned (vì chỉ lo trường hợp chuyển về Planned)
          .When(cmd => cmd.NewStatus == ScheduleStatus.Planned, ApplyConditionTo.CurrentValidator);



        //// Tất cả các rule bên trong khối When này chỉ chạy NẾU IsSpecialProduct là true
        //When(cmd => cmd.IsSpecialProduct, () => {
        //    RuleFor(x => x.SpecialNotes)
        //        .NotEmpty().WithMessage("Special notes are required for special products.")
        //        .MaximumLength(500).WithMessage("Special notes for special products cannot exceed 500 characters.");

        //    RuleFor(x => x.Price)
        //        .GreaterThan(50).WithMessage("Price for special products must be greater than 50.");
        //    // Bất kỳ rule nào khác bạn thêm vào đây cũng sẽ chịu sự chi phối của When(cmd => cmd.IsSpecialProduct)
        //});

        //// Rule này nằm ngoài khối When, nên nó luôn chạy
        //RuleFor(x => x.SpecialNotes)
        //    .Must(notes => notes == null || !notes.Contains("forbidden_word"))
        //    .WithMessage("Special notes cannot contain 'forbidden_word'.");
    }

    /// <summary>
    /// Kiểm tra xem việc chuyển đổi trạng thái có hợp lệ không.
    /// Cụ thể: Không cho phép chuyển về 'Planned' nếu trạng thái hiện tại là 'Eaten' hoặc 'Skipped'.
    /// </summary>
    private async Task<bool> BeValidStatusTransition(UpdateScheduledMealStatusCommand command, CancellationToken cancellationToken)
    {
        // Chỉ kiểm tra nếu NewStatus là Planned
        if (command.NewStatus != ScheduleStatus.Planned)
        {
            return true; // Các chuyển đổi khác được cho là hợp lệ (hoặc bạn có thể thêm rule khác)
        }

        // Lấy trạng thái hiện tại của ScheduledMeal từ database
        var scheduledMeal = await _unitOfWork.ScheduledMeals.GetByIdAsync(command.ScheduleId, cancellationToken);

        if (scheduledMeal == null)
        {
            // Nếu không tìm thấy, rule này không áp dụng (rule khác sẽ bắt lỗi không tìm thấy)
            // Hoặc bạn có thể trả về false nếu muốn:
            // return false; // -> sẽ hiển thị thông báo lỗi của rule này
            return true; // Để cho rule khác (ví dụ: ScheduleId phải tồn tại) xử lý
        }

        // Kiểm tra logic: Nếu trạng thái hiện tại là Eaten hoặc Skipped, và NewStatus là Planned -> không hợp lệ
        if ((scheduledMeal.Status == ScheduleStatus.Eaten || scheduledMeal.Status == ScheduleStatus.Skipped) &&
            command.NewStatus == ScheduleStatus.Planned)
        {
            return false; // Không cho phép chuyển đổi này
        }

        return true; // Các trường hợp khác là hợp lệ
    }
}
