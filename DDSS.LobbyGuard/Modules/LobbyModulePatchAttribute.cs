using System;

namespace DDSS_LobbyGuard.Modules
{
    internal class LobbyModulePatchAttribute
        : Attribute
    {
        internal readonly Type type;
        internal LobbyModulePatchAttribute(Type type)
            => this.type = type;
    }
}
