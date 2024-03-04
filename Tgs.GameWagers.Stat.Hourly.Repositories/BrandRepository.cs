using Microsoft.EntityFrameworkCore;
using Tgs.GameWagers.Stat.Hourly.Core;
using Tgs.GameWagers.Stat.Hourly.Core.Models;
using Tgs.GameWagers.Stat.Hourly.Entities;

namespace Tgs.GameWagers.Stat.Hourly.Repositories;

internal class BrandRepository(
	TgsContext context) : IBrandRepository
{
	public async ValueTask<BrandInfo?> GetAsync(string code, CancellationToken cancellationToken = default)
	=> await context.Brands
		.Join(
		context.Licensees,
		brand => brand.LicenseeId,
		licensee => licensee.Id,
		(brand, licensee) => new
		{
			licensee.ResellerId,
			LicenseeId = licensee.Id,
			LicenseeCode = licensee.Code,
			LicenseeStatus = licensee.Status,
			BrandId = brand.Id,
			BrandCode = brand.Code,
			BrandStatus = brand.Status,
			TimeSpanMinutes = brand.TimeSpanMinutes.GetValueOrDefault(480)
		})
		.Join(
		context.Resellers,
		conform => conform.ResellerId,
		reseller => reseller.Id,
		(conform, reseller) => new
		{
			conform.ResellerId,
			ResellerCode = reseller.Code,
			ResellerStatus = reseller.Status,
			conform.LicenseeId,
			conform.LicenseeCode,
			conform.LicenseeStatus,
			conform.BrandId,
			conform.BrandCode,
			conform.BrandStatus,
			conform.TimeSpanMinutes
		})
		.Where(predicate: conform => conform.BrandCode == code)
		.Select(selector: conform => new BrandInfo(
			conform.ResellerId,
			conform.ResellerCode,
			conform.LicenseeId,
			conform.LicenseeCode,
			conform.BrandId,
			conform.BrandCode,
			conform.TimeSpanMinutes
			))
		.AsAsyncEnumerable()
		.FirstOrDefaultAsync(
		cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public IAsyncEnumerable<BrandInfo> QueryAsync(CancellationToken cancellationToken = default)
	=> context.Brands
		.Join(
		context.Licensees,
		brand => brand.LicenseeId,
		licensee => licensee.Id,
		(brand, licensee) => new
		{
			licensee.ResellerId,
			LicenseeId = licensee.Id,
			LicenseeCode = licensee.Code,
			LicenseeStatus = licensee.Status,
			BrandId = brand.Id,
			BrandCode = brand.Code,
			BrandStatus = brand.Status,
			TimeSpanMinutes = brand.TimeSpanMinutes.GetValueOrDefault(480)
		})
		.Join(
		context.Resellers,
		conform => conform.ResellerId,
		reseller => reseller.Id,
		(conform, reseller) => new
		{
			conform.ResellerId,
			ResellerCode = reseller.Code,
			ResellerStatus = reseller.Status,
			conform.LicenseeId,
			conform.LicenseeCode,
			conform.LicenseeStatus,
			conform.BrandId,
			conform.BrandCode,
			conform.BrandStatus,
			conform.TimeSpanMinutes
		})
		.Where(predicate: conform => conform.ResellerStatus != 2 && conform.LicenseeStatus != 2 && conform.BrandStatus != 2)
		.Select(selector: conform => new BrandInfo(
			conform.ResellerId,
			conform.ResellerCode,
			conform.LicenseeId,
			conform.LicenseeCode,
			conform.BrandId,
			conform.BrandCode,
			conform.TimeSpanMinutes))
		.AsAsyncEnumerable();
}
