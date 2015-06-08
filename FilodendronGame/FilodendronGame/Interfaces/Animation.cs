using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FilodendronGame.Interfaces
{
    public interface Animation
    {
        Matrix World { get; set; }
        Matrix UpdateAnimation();
        //Vector3 UpdateAnimation();
        /*Vector3 UpdateAnimationX();
        Vector3 UpdateAnimationY();
        Vector3 UpdateAnimationZ();
        void savePositionDirection();
        void IsAnimationStatus();
        Vector3 AnimationBackwardZ();
        Vector3 AnimationForwardZ();
        Vector3 AnimationDownY();
        Vector3 AnimationUpY();
        Vector3 AnimationRightX();
        Vector3 AnimationLeftX();*/
    }
}
