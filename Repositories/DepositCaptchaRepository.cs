using System.Collections.Generic;
using BusinessObjects.Models;
using DataAccessObjects;

namespace Repositories;

public class DepositCaptchaRepository : IDepositCaptchaRepository
{
    public IEnumerable<DepositCaptcha> GetAllCaptchas() => DepositCaptchaDAO.Instance.GetAllCaptchas();

    public DepositCaptcha? GetCaptchaById(int id) => DepositCaptchaDAO.Instance.GetCaptchaById(id);

    public void AddCaptcha(DepositCaptcha captcha) => DepositCaptchaDAO.Instance.AddCaptcha(captcha);

    public DepositCaptcha GenerateCaptcha(int carId, string? code) => DepositCaptchaDAO.Instance.GenerateCaptcha(carId, code);
}
