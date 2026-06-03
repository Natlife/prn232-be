using System.Collections.Generic;
using BusinessObjects.Models;

namespace Repositories;

public interface IDepositCaptchaRepository
{
    IEnumerable<DepositCaptcha> GetAllCaptchas();
    DepositCaptcha? GetCaptchaById(int id);
    void AddCaptcha(DepositCaptcha captcha);
    DepositCaptcha GenerateCaptcha(int carId, string? code);
}
