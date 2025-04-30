using Application.Features.Constants;
using Application.Interfaces;
using Application.Interfaces.Application;
using Application.Responses;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class DemoService : IDemoService<DemoDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DemoService> _logger;
        private readonly IAsyncRepository<Demo> _demoRepository;
        public DemoService(IUnitOfWork unitOfWork, ILogger<DemoService> logger, IAsyncRepository<Demo> demoRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _demoRepository = demoRepository;
        }

        public async Task<BaseResponse<IEnumerable<DemoDTO>>> GetDemo()
        {
            try
            {
                // Code to retrieve all merchants from the database
                var demos = await _demoRepository.WhereQueryable(x => x.IsDeleted == false);
                List<DemoDTO> demoDTOs = new List<DemoDTO>();
                foreach (var demo in demos)
                {
                    DemoDTO demoDTO = new DemoDTO();
                    demoDTO.ConvertToDTO(demo);
                    demoDTOs.Add(demoDTO);
                }

                return new BaseResponse<IEnumerable<DemoDTO>>("Retrieved all demos successfully", demoDTOs, ResponseCodes.SUCCESS);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving list of Demo.");
                return new BaseResponse<IEnumerable<DemoDTO>>($"An error occurred while retrieving list of Demo. {ex.Message})");
            }
        }

        public async Task<BaseResponse<DemoDTO>> GetDemoById(string id)
        {
            try
            {
                var demo = await _demoRepository.GetByIdAsync(id);

                if (demo == null)
                {
                    return new BaseResponse<DemoDTO>("Demo not found", ResponseCodes.NOT_FOUND);
                }

                DemoDTO demoDTO = new DemoDTO();
                demoDTO.ConvertToDTO(demo);

                return new BaseResponse<DemoDTO>("Demo returned successfully", demoDTO, ResponseCodes.SUCCESS);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving Demo with {id}.");
                return new BaseResponse<DemoDTO>($"An error occurred while retrieving Demo with {id}.", ex.Message);
            }
        }

        public async Task<BaseResponse<DemoDTO>> CreateDemo(DemoDTO demoDTO)
        {
            try
            {
                var demo = new Demo
                {
                    Status = "Active",
                    CreatedBy = "System",
                    CreatedAt = DateTime.Now,
                };

                demoDTO.ConvertFromDTO(demo);
                var checkName = CreatePrevalidationChecks(demo);
                if (checkName.Item1 == false)
                {
                    return new BaseResponse<DemoDTO>(checkName.Item2, ResponseCodes.DUPLICATE_RESOURCE);
                }
                var createdDemo = _demoRepository.AddAsync(demo);
                await _unitOfWork.CommitChangesAsync();

                demoDTO.ConvertToDTO(createdDemo.Result);

                return new BaseResponse<DemoDTO>("Demo Created Successfully", demoDTO, ResponseCodes.CREATED);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while Demo.");
                return new BaseResponse<DemoDTO>("An error occurred while Demo", ex.Message);
            }
        }

        public async Task<BaseResponse<DemoDTO>> UpdateDemo(DemoDTO demoDTO)
        {
            try
            {
                var demo = new Demo
                {
                    Status = "Active",
                    LastUpdatedBy = "System",
                    LastUpdatedAt = DateTime.Now,
                };

                demoDTO.ConvertFromDTO(demo);
                // Code to update a merchant in the database

                var existingObject = UpdatePrevalidationChecks(demo);
                if (existingObject.Item1)
                {
                    return new BaseResponse<DemoDTO>(existingObject.Item2, ResponseCodes.DUPLICATE_RESOURCE);
                }

                var updatedMerchant = await _demoRepository.UpdateAsync(demo);
                await _unitOfWork.CommitAsync();

                demoDTO.ConvertToDTO(updatedMerchant);

                return new BaseResponse<DemoDTO>("Demo updated successfully", demoDTO, ResponseCodes.UPDATED);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while Updating Demo with {demoDTO.Id}.");
                return new BaseResponse<DemoDTO>($"An error occurred while updating Demo with {demoDTO.Id}.", ex.Message);
            }
        }

        public async Task<BaseResponse<DemoDTO>> DeleteDemo(string id)
        {
            try
            {
                var demo = await _demoRepository.GetByIdAsync(id);
                if (demo == null)
                {
                    _logger.LogInformation("Demo not found. ID: {ID}", id);
                    return new BaseResponse<DemoDTO>("Demo not found", ResponseCodes.NOT_FOUND);
                }

                demo.IsDeleted = true;
                demo.Status = "InActive";

                var existingObject = DeletePrevalidationChecks(id);
                if (existingObject.Item1 == false)
                {
                    return new BaseResponse<DemoDTO>(existingObject.Item2, ResponseCodes.DUPLICATE_RESOURCE);
                }
                var returned = await _demoRepository.UpdateAsync(demo);
                await _unitOfWork.CommitAsync();

                DemoDTO demoDTO = new DemoDTO();
                demoDTO.ConvertToDTO(returned);

                _logger.LogInformation("Demo deleted successfully. ID: {ID}", id);
                return new BaseResponse<DemoDTO>("Demo deleted successfully", demoDTO, ResponseCodes.DELETED);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting Demo with {id}.");
                return new BaseResponse<DemoDTO>($"An error occurred while deleting Demo with {id}.", ex.Message);
            }
        }

        private (bool, string) CreatePrevalidationChecks(Demo demo)
        {
            Demo existingDemo = new Demo();
            existingDemo = _demoRepository.SingleOrDefaultAsync(x => x.Name == demo.Name || x.Status != "Deleted").Result;
            if (existingDemo != null)
            {
                return (false, "Name already exist");
            }
            return (true, string.Empty);
        }

        private (bool, string) UpdatePrevalidationChecks(Demo demo)
        {
            Demo existingDemo = new Demo();
            existingDemo = _demoRepository.SingleOrDefaultAsync(x => x.Id == demo.Id).Result;
            if (existingDemo == null)
            {
                return (false, $"Object with {demo.Id} Doesn't Exist");
            }
            return (true, demo.Id);
        }

        private (bool, string) DeletePrevalidationChecks(string id)
        {
            Demo existingDemo = new Demo();
            existingDemo = _demoRepository.SingleOrDefaultAsync(x => x.Id == id).Result;
            if (existingDemo.IsDeleted)
            {
                return (false, $"Object with {id} has been deleted previously");
            }
            return (true, id);
        }
    }
}
