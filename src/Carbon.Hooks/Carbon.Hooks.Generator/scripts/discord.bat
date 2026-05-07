@echo off

if %DEBUG_BUILD% EQU true if %RELEASE_BUILD% EQU true (
	call :create_output %2 "debug+release" %1
) else (
	if %DEBUG_BUILD% EQU true (
		call :create_output %2 debug %1
	)
	
	if %RELEASE_BUILD% EQU true (
		call :create_output %2 release %1
	)
)

curl -H "Content-Type: application/json" -d @discord_output.json %DISCORD_HOOK_WH%

:create_output

if %DEBUG_BUILD% EQU true if %RELEASE_BUILD% EQU true (

echo {^
  "content": null,^
  "embeds": [^
    {^
      "title": "Hook Update",^
      "description": "**New protocol hook update available!**\nRestart the server with the same protocol to update.",^
      "url": "%ACTION_URL%",^
      "color": 8443463,^
      "fields": [^
        {^
          "name": "Protocol",^
          "value": "%~1",^
          "inline": true^
        },^
        {^
          "name": "Type",^
          "value": "%~2",^
          "inline": true^
        },^
        {^
          "name": "Rust",^
          "value": "%~3",^
          "inline": true^
        },^
        {^
          "name": "Oxide Hooks",^
          "value": "[Rust.opj](%OXIDE_HOOKS%)"^
        },^
        {^
          "name": "Download Debug",^
          "value": "Windows:\n[Carbon.Hooks.Community.dll](https://cdn.carbonmod.gg/hooks/server/debug/%~1/carbon/managed/hooks/Carbon.Hooks.Community.dll)\n[Carbon.Hooks.Oxide.dll](https://cdn.carbonmod.gg/hooks/server/debug/%~1/carbon/managed/hooks/Carbon.Hooks.Oxide.dll)\nUnix:\n[Carbon.Hooks.Community.dll](https://cdn.carbonmod.gg/hooks/server/debugunix/%~1/carbon/managed/hooks/Carbon.Hooks.Community.dll)\n[Carbon.Hooks.Oxide.dll](https://cdn.carbonmod.gg/hooks/server/debugunix/%~1/carbon/managed/hooks/Carbon.Hooks.Oxide.dll)",^
          "inline": true^
        },^
        {^
          "name": "Download Release",^
          "value": "Windows:\n[Carbon.Hooks.Community.dll](https://cdn.carbonmod.gg/hooks/server/release/%~1/carbon/managed/hooks/Carbon.Hooks.Community.dll)\n[Carbon.Hooks.Oxide.dll](https://cdn.carbonmod.gg/hooks/server/release/%~1/carbon/managed/hooks/Carbon.Hooks.Oxide.dll)\nUnix:\n[Carbon.Hooks.Community.dll](https://cdn.carbonmod.gg/hooks/server/releaseunix/%~1/carbon/managed/hooks/Carbon.Hooks.Community.dll)\n[Carbon.Hooks.Oxide.dll](https://cdn.carbonmod.gg/hooks/server/releaseunix/%~1/carbon/managed/hooks/Carbon.Hooks.Oxide.dll)",^
          "inline": true^
        }^
      ],^
	  "thumbnail": {^
        "url": "https://cdn.carbonmod.gg/carbonvector_go.png"^
      }^
    }^
  ],^
  "attachments": []^
} > discord_output.json

) ELSE (

	if %DEBUG_BUILD% EQU true (
	
		echo {^
  "content": null,^
  "embeds": [^
    {^
      "title": "Hook Update",^
      "description": "**New protocol hook update available!**\nRestart the server with the same protocol to update.",^
      "url": "%ACTION_URL%",^
      "color": 8443463,^
      "fields": [^
        {^
          "name": "Protocol",^
          "value": "%~1",^
          "inline": true^
        },^
        {^
          "name": "Type",^
          "value": "%~2",^
          "inline": true^
        },^
        {^
          "name": "Rust",^
          "value": "%~3",^
          "inline": true^
        },^
        {^
          "name": "Oxide Hooks",^
          "value": "[Rust.opj](%OXIDE_HOOKS%)"^
        },^
        {^
          "name": "Download",^
          "value": "Windows:\n[Carbon.Hooks.Community.dll](https://cdn.carbonmod.gg/hooks/server/debug/%~1/carbon/managed/hooks/Carbon.Hooks.Community.dll)\n[Carbon.Hooks.Oxide.dll](https://cdn.carbonmod.gg/hooks/server/debug/%~1/carbon/managed/hooks/Carbon.Hooks.Oxide.dll)\nUnix:\n[Carbon.Hooks.Community.dll](https://cdn.carbonmod.gg/hooks/server/debugunix/%~1/carbon/managed/hooks/Carbon.Hooks.Community.dll)\n[Carbon.Hooks.Oxide.dll](https://cdn.carbonmod.gg/hooks/server/debugunix/%~1/carbon/managed/hooks/Carbon.Hooks.Oxide.dll)",^
          "inline": true^
        }^
      ],^
	  "thumbnail": {^
        "url": "https://carbonmod.gg/assets/media/carbonvector_go.png"^
      }^
    }^
  ],^
  "attachments": []^
} > discord_output.json
	
	)
	
	if %RELEASE_BUILD% EQU true (
	
		echo {^
  "content": null,^
  "embeds": [^
    {^
      "title": "Hook Update",^
      "description": "**New protocol hook update available!**\nRestart the server with the same protocol to update.",^
      "url": "%ACTION_URL%",^
      "color": 8443463,^
      "fields": [^
        {^
          "name": "Protocol",^
          "value": "%~1",^
          "inline": true^
        },^
        {^
          "name": "Type",^
          "value": "%~2",^
          "inline": true^
        },^
        {^
          "name": "Rust",^
          "value": "%~3",^
          "inline": true^
        },^
        {^
          "name": "Oxide Hooks",^
          "value": "[Rust.opj](%OXIDE_HOOKS%)"^
        },^
        {^
          "name": "Download",^
          "value": "Windows:\n[Carbon.Hooks.Community.dll](https://cdn.carbonmod.gg/hooks/server/release/%~1/carbon/managed/hooks/Carbon.Hooks.Community.dll)\n[Carbon.Hooks.Oxide.dll](https://cdn.carbonmod.gg/hooks/server/release/%~1/carbon/managed/hooks/Carbon.Hooks.Oxide.dll)\nUnix:\n[Carbon.Hooks.Community.dll](https://cdn.carbonmod.gg/hooks/server/releaseunix/%~1/carbon/managed/hooks/Carbon.Hooks.Community.dll)\n[Carbon.Hooks.Oxide.dll](https://cdn.carbonmod.gg/hooks/server/releaseunix/%~1/carbon/managed/hooks/Carbon.Hooks.Oxide.dll)",^
          "inline": true^
        }^
      ],^
	  "thumbnail": {^
        "url": "https://carbonmod.gg/assets/media/carbonvector_go.png"^
      }^
    }^
  ],^
  "attachments": []^
} > discord_output.json
	
	)

)
