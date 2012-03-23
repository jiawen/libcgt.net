using libcgt.core.Vecmath;

namespace libcgt.core.Trackball
{
    public class OldCamera : Camera
    {
        private float zNear;
        private float zFar;

        public OldCamera( Vector3f position, Vector3f forward, Vector3f up,
            float fovYDegrees,
            float zNear, float zFar,
            int screenWidth, int screenHeight )
            : base( position, forward, up, fovYDegrees, screenWidth, screenHeight )
        {
            this.zNear = zNear;
            this.zFar = zFar;
        }

        public OldCamera( Camera camera )
            : base( camera )
        {
            zNear = camera.ZNear;
            zFar = camera.ZFar;
        }

        public override float ZNear
        {
            get
            {
                return zNear;
            }
            set
            {
                zNear = value;
            }
        }

        public override float ZFar
        {
            get
            {
                return zFar;
            }
            set
            {
                zFar = value;
            }
        }

        public override void Set( Camera camera )
        {
            base.Set( camera );

            zNear = camera.ZNear;
            zFar = camera.ZFar;
        }
    }
}
