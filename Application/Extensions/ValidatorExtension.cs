﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class ValidatorExtension
    {
        public static bool IsAtLeastNYearsOld(DateTime? dateOfBirth, int year)
        {
            DateTime currentDate = DateTime.Now;
            DateTime minimumBirthDate = currentDate.AddYears(-year);
            return dateOfBirth <= minimumBirthDate;
        }

        public static bool BeValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return Regex.IsMatch(email, pattern);
        }
        public static bool IsEqualOrAfterDay(DateTime? time, DateTime day)
        {
            return time >= day;
        }


        public static bool IsAfterDay(DateTime? time, DateTime day)
        {
            return time > day;
        }

        public static bool IsImage(IFormFile? file)
        {
            if (file == null) return true; // Null is allowed
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }
        public static bool IsValidFile(string file)
        {
            string pattern = @"^[^.]+\.[a-zA-Z]+$";

            return Regex.IsMatch(file, pattern) && file.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }
    }
    }
