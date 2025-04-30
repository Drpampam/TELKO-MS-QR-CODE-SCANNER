using Application.Responses;
using Domain.DTOs;

namespace Application.Interfaces.Application
{
    public interface IDemoService<T> where T : class
    {
        Task<BaseResponse<IEnumerable<T>>> GetDemo();
        Task<BaseResponse<T>> GetDemoById(string id);
        Task<BaseResponse<T>> CreateDemo(DemoDTO createDemoDTO);
        Task<BaseResponse<T>> UpdateDemo(DemoDTO updateDemoDTO);
        Task<BaseResponse<T>> DeleteDemo(string id);
    }
}
