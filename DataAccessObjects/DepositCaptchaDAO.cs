using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class DepositCaptchaDAO
{
    private static DepositCaptchaDAO? _instance = null;
    private static readonly object _instanceLock = new object();

    private DepositCaptchaDAO() { }

    public static DepositCaptchaDAO Instance
    {
        get
        {
            lock (_instanceLock)
            {
                if (_instance == null)
                {
                    _instance = new DepositCaptchaDAO();
                }
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
        context.DepositCaptchas.Add(captcha);
        context.SaveChanges();
    }

    public DepositCaptcha GenerateCaptcha(int carId, string? code)
    {
        using var context = new CarShowroomContext();
        
        var carExists = context.Cars.Any(c => c.CarId == carId);
        if (!carExists)
        {
            throw new ArgumentException("Xe kh?ng t?n t?i.");
        }

        string finalCode = string.IsNullOrEmpty(code) ? GenerateRandomCode(context) : code.Trim().ToUpper();

        var exists = context.DepositCaptchas.Any(c => c.Code == finalCode);
        if (exists)
        {
            if (!string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("M? x?c nh?n n?y d? t?n t?i trong h? th?ng.");
            }
            else
            {
                finalCode = GenerateRandomCode(context);
            }
        }

        var captcha = new DepositCaptcha
        {
            Code = finalCode,
            CarId = carId,
            IsUsed = false,
            CreatedAt = DateTime.Now
        };

        context.DepositCaptchas.Add(captcha);
        context.SaveChanges();

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
