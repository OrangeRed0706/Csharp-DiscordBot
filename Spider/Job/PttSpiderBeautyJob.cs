using Spider.Services;

namespace Spider.Job;

public class PttSpiderBeautyJob
{
    private PttSpiderService _PttSpiderService { get; set; }
    public PttSpiderBeautyJob(PttSpiderService pttSpiderService)
    {
        _PttSpiderService = pttSpiderService;
    }
    
}