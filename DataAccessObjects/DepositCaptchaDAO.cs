using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class DepositCaptchaDAO
{
    private static DepositCaptchaDAO? _instance;
    private static readonly object _instanceLock = new();

    private DepositCaptchaDAO() { }

    public static DepositCaptchaDAO Instance
    {
        get
        {
            lock (_instanceLock)
            {
                _instance ??= new DepositCaptchaDAO();
                return _instance;
            }
        }
    }

    public IEnumerable<DepositCaptcha> GetAllCaptchas()
    {
        using var context = new CarShowroomContext();
        return context.DepositCaptchas.Include(c => c.Car).ToList();
    }

    public DepositCaptcha? GetCaptchaById(int id)
    {
        using var context = new CarShowroomContext();
        return context.DepositCaptchas.Include(c => c.Car).SingleOrDefault(c => c.CaptchaId == id);
    }

    public void AddCaptcha(DepositCaptcha captcha)
    {
        using var context = new CarShowroomContext();

        try
        {
            context.DepositCaptchas.Add(captcha);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ExceptionMessageHelper.GetDetailedMessage(ex), ex);
        }
    }

    public DepositCaptcha GenerateCaptcha(int carId, string? code)
    {
        using var context = new CarShowroomContext();

        var carExists = context.Cars.Any(c => c.CarId == carId);
        if (!carExists)
        {
            throw new ArgumentException("Xe khong ton tai.");
        }

        var finalCode = string.IsNullOrWhiteSpace(code) ? GenerateRandomCode(context) : code.Trim().ToUpperInvariant();
        var exists = context.DepositCaptchas.Any(c => c.Code == finalCode);
        if (exists)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("Ma xac nhan nay da ton tai trong he thong.");
            }

            finalCode = GenerateRandomCode(context);
        }

        var captcha = new DepositCaptcha
        {
            Code = finalCode,
            CarId = carId,
            IsUsed = false,
            CreatedAt = DateTime.Now
        };

        try
        {
            context.DepositCaptchas.Add(captcha);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ExceptionMessageHelper.GetDetailedMessage(ex), ex);
        }

        captcha.Car = context.Cars.Single(c => c.CarId == carId);
        return captcha;
    }

    private string GenerateRandomCode(CarShowroomContext context)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code;

        do
        {
            code = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        } while (context.DepositCaptchas.Any(c => c.Code == code));

        return code;
    }
}
