﻿namespace Microservices.App.Services.WebAPI.Models.Dtos
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public double DicountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}