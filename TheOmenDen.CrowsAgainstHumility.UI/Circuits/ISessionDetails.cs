﻿using System.Security.Claims;
using System.Security.Principal;
using TheOmenDen.CrowsAgainstHumility.Events;

namespace TheOmenDen.CrowsAgainstHumility.Circuits;
public interface ISessionDetails
{
    void ConnectSession(SessionModel session, String userName);

    void ConnectSession(String circuitId, IIdentity user);

    void ConnectSession(string circuitId, ClaimsPrincipal user);

    void DisconnectSession(String? circuitId);

    event EventHandler CircuitsChanged;

    event UserDisconnectEventHandler UserDisconnect;
}
