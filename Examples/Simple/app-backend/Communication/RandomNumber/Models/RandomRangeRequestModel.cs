using Tapper;

namespace App_backend.Communication.RandomNumber.Models
{
    [TranspilationSource]
    public class RandomRangeRequestModel
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
}
