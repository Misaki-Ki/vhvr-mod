if not exist  "%GAME_DIR%BepInEx\plugins" mkdir  "%GAME_DIR%BepInEx\plugins"
copy "%TARGET_PATH%" "%GAME_DIR%BepInEx\plugins"
if not exist "%GAME_DIR%BepInEx\plugins\bHaptics" mkdir "%GAME_DIR%BepInEx\plugins\bHaptics"
Xcopy /Y /E /I "%SOLUTION_DIR%bHaptics" "%GAME_DIR%BepInEx\plugins\bHaptics"
if not exist "%GAME_DIR%Valheim_Data\Managed" mkdir "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\SteamVR.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\SteamVR_Actions.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\Unity.XR.Management.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\Unity.XR.OpenVR.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\netstandard.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\UnityEngine.XR.LegacyInputHelpers.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\UnityEngine.SpatialTracking.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\amplify_occlusion.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\final_ik.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\root_motion_demo_assets.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\root_motion_shared.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\Valve.Newtonsoft.Json.dll" "%GAME_DIR%Valheim_Data\Managed"
copy "%TARGET_DIR%Bhaptics.Tact.dll" "%GAME_DIR%Valheim_Data\Managed" 
copy "%TARGET_DIR%NDesk.Options.dll" "%GAME_DIR%Valheim_Data\Managed" 

if not exist "%GAME_DIR%Valheim_Data\Plugins\x86_64" mkdir "%GAME_DIR%Valheim_Data\Plugins\x86_64"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Plugins\x86_64\XRSDKOpenVR.dll" "%GAME_DIR%Valheim_Data\Plugins\x86_64"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Plugins\x86_64\openvr_api.dll" "%GAME_DIR%Valheim_Data\Plugins\x86_64"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Plugins\x86_64\ucrtbased.dll" "%GAME_DIR%Valheim_Data\Plugins\x86_64"

if not exist "%GAME_DIR%Valheim_Data\UnitySubsystems" mkdir "%GAME_DIR%Valheim_Data\UnitySubsystems"
if not exist "%GAME_DIR%Valheim_Data\UnitySubsystems\XRSDKOpenVR" mkdir "%GAME_DIR%Valheim_Data\UnitySubsystems\XRSDKOpenVR"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\UnitySubsystems\XRSDKOpenVR\UnitySubsystemsManifest.json" "%GAME_DIR%Valheim_Data\UnitySubsystems\XRSDKOpenVR"

if not exist  "%GAME_DIR%Valheim_Data\StreamingAssets\SteamVR" mkdir  "%GAME_DIR%Valheim_Data\StreamingAssets\SteamVR"
Xcopy /Y /E /I "%SOLUTION_DIR%Unity\build\ValheimVR_Data\StreamingAssets\SteamVR"  "%GAME_DIR%Valheim_Data\StreamingAssets\SteamVR"

if not exist "%GAME_DIR%Valheim_Data\StreamingAssets" mkdir "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\AssetBundles"  "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\AssetBundles.manifest"  "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\steamvr_shaders"  "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\steamvr_shaders.manifest"  "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\xrmanager"  "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\xrmanager.manifest"  "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\steamvr_player_prefabs" "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\steamvr_player_prefabs.manifest"  "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\vhvr_custom" "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\vhvr_custom.manifest"  "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\amplify_resources" "%GAME_DIR%Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\amplify_resources.manifest"  "%GAME_DIR%Valheim_Data\StreamingAssets"

if not exist "%SOLUTION_DIR%ValheimVRMod\release" mkdir "%SOLUTION_DIR%ValheimVRMod\release"
if not exist "%SOLUTION_DIR%ValheimVRMod\release\BepInEx" mkdir "%SOLUTION_DIR%ValheimVRMod\release\BepInEx"
if not exist "%SOLUTION_DIR%ValheimVRMod\release\BepInEx\plugins" mkdir  "%SOLUTION_DIR%ValheimVRMod\release\BepInEx\plugins"
copy "%TARGET_PATH%" "%SOLUTION_DIR%ValheimVRMod\release\BepInEx\plugins"
if not exist "%GAME_DIR%BepInEx\plugins\bHaptics" mkdir "%GAME_DIR%BepInEx\plugins\bHaptics"
Xcopy /Y /E /I "%SOLUTION_DIR%bHaptics" "%SOLUTION_DIR%ValheimVRMod\release\BepInEx\plugins\bHaptics"

if not exist "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data" mkdir "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data"
if not exist "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed" mkdir "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\SteamVR.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\SteamVR_Actions.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\Unity.XR.Management.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\Unity.XR.OpenVR.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\netstandard.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\UnityEngine.XR.LegacyInputHelpers.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\UnityEngine.SpatialTracking.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\amplify_occlusion.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\final_ik.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\root_motion_demo_assets.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\root_motion_shared.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Managed\Valve.Newtonsoft.Json.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%TARGET_DIR%Bhaptics.Tact.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"
copy "%TARGET_DIR%NDesk.Options.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Managed"

if not exist "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Plugins" mkdir if not exist "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Plugins"
if not exist "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Plugins\x86_64" mkdir "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Plugins\x86_64"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Plugins\x86_64\XRSDKOpenVR.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Plugins\x86_64"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Plugins\x86_64\openvr_api.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Plugins\x86_64"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\Plugins\x86_64\ucrtbased.dll" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\Plugins\x86_64"

if not exist "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\UnitySubsystems" mkdir "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\UnitySubsystems"
if not exist "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\UnitySubsystems\XRSDKOpenVR" mkdir "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\UnitySubsystems\XRSDKOpenVR"
copy "%SOLUTION_DIR%Unity\build\ValheimVR_Data\UnitySubsystems\XRSDKOpenVR\UnitySubsystemsManifest.json" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\UnitySubsystems\XRSDKOpenVR"

if not exist "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets" mkdir "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
if not exist "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets\SteamVR" mkdir  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets\SteamVR"
Xcopy /Y /E /I "%SOLUTION_DIR%Unity\build\ValheimVR_Data\StreamingAssets\SteamVR"  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets\SteamVR"

copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\AssetBundles"  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\AssetBundles.manifest"  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\steamvr_shaders"  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\steamvr_shaders.manifest"  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\xrmanager"  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\xrmanager.manifest"  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\steamvr_player_prefabs" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\steamvr_player_prefabs.manifest"  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\vhvr_custom" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\vhvr_custom.manifest"  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\amplify_resources" "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"
copy  "%SOLUTION_DIR%Unity\ValheimVR\Assets\AssetBundles\amplify_resources.manifest"  "%SOLUTION_DIR%ValheimVRMod\release\Valheim_Data\StreamingAssets"

::: Interactive Editor Debugging :::

if exist "%UNITY_DIR%" if not exist "%GAME_DIR%WinPixEventRuntime.dll" (
    :: this part is needed only once, it will turn the game into a development build  
    Xcopy "%UNITY_DIR%Editor\Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_development_mono\Data" "%GAME_DIR%valheim_Data" /s /y /i
    copy "%UNITY_DIR%Editor\Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_development_mono\WindowsPlayer.exe" "%GAME_DIR%valheim.exe"
    copy "%UNITY_DIR%Editor\Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_development_mono\UnityPlayer.dll" "%GAME_DIR%"
    copy "%UNITY_DIR%Editor\Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_development_mono\WinPixEventRuntime.dll" "%GAME_DIR%"
)

: this part is needed for every build
"%SOLUTION_DIR%pdb2mdb.exe" "%TARGET_PATH%"
Xcopy "%TARGET_DIR%ValheimVRMod.dll.mdb" "%GAME_DIR%Bepinex\plugins" /s /y /i