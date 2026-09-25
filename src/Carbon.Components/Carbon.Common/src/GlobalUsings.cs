global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.Serialization;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using Carbon;
global using Carbon.Base;
global using Carbon.Components;
global using Carbon.Core;
global using Carbon.Events;
global using Carbon.Extensions;
global using Carbon.Hooks;
global using Carbon.Managers;
global using Carbon.Modules;
global using Carbon.Plugins;
global using Carbon.Pooling;
global using UnityEngine;
global using Timer = Carbon.Plugins.Timer;
using static Carbon.Components.MonoProfiler;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Carbon.Hooks.Base")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Carbon")]
