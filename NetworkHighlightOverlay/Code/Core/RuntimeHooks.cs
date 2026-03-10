using System;

namespace NetworkHighlightOverlay.Code.Core
{
    public static class RuntimeHooks
    {
        private static Manager _manager;

        public static void Attach(Manager manager)
        {
            _manager = manager ?? throw new ArgumentNullException("manager");
        }

        public static void Detach()
        {
            _manager = null;
        }

        public static void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            Manager manager = _manager;
            if (manager == null)
                return;

            manager.RenderIfActive(cameraInfo);
        }

        public static void HandleSegmentCreated(ushort segment, NetInfo info, bool creationSucceeded)
        {
            Manager manager = _manager;
            if (manager == null || !creationSucceeded || segment == 0 || info == null || info.m_netAI == null)
                return;

            manager.OnSegmentCreated(segment);
        }

        public static void HandleSegmentReleased(ushort segment)
        {
            Manager manager = _manager;
            if (manager == null || segment == 0)
                return;

            manager.OnSegmentReleased(segment);
        }
    }
}
