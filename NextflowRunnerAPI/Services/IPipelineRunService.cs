﻿using NextflowRunnerAPI.Models;

namespace NextflowRunnerAPI.Services
{
    public interface IPipelineRunService
    {
        Task<List<PipelineRun>> GetPipelineRunsAsync(int pipelineId);
        Task<PipelineRun?> GetPipelineRunAsync(int PipelineRunId);
        Task<PipelineRun?> CreatePipelineRunAsync(int pipelineId, PipelineRun PipelineRun);
        Task<PipelineRun?> UpdatePipelineRunAsync(PipelineRun PipelineRun);
        Task DeletePipelineRunAsync(int PipelineRunId);
    }
}