using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionFiller.Helpers
{
    public static class UserInfoListenersNotifier
    {
        private static List<UserInfoListener> listeners = new List<UserInfoListener>();
        public static void addListener(UserInfoListener listener)
        {
            listeners.Add(listener);
        }
        public static void notifyListeners()
        {
            foreach (UserInfoListener listener in listeners)
            {
                listener.update();
            }
        }
    }
}
