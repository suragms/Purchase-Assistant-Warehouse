using PurchaseAssistant.Application.DTOs.Dashboard;
using System.Threading.Tasks;

namespace PurchaseAssistant.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();
    }
}
