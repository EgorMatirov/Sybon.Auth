using JetBrains.Annotations;

namespace Sybon.Auth.ApiStubs
{
    public interface IProblemsApi
    {
        Problem GetById(long problemId);
    }

    [UsedImplicitly]
    public class Problem 
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string StatementUrl { get; set; }
        public long? CollectionId { get; set; }
        public int? TestsCount { get; set; }
        public int? PretestsCount { get; set; }
        public string InternalProblemId { get; set; }
//        public ResourceLimits ResourceLimits { get; set; }
    }
}