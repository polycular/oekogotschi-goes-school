using System;

namespace EcoGotchi
{
	public interface IGameDirector 
	{
		event Action GameCompleted;
		ImageTargetTracker ImgTargetTracker { get; set; }
	}
}
