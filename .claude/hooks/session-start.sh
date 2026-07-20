#!/bin/bash
# SessionStart hook for Claude Code on the web.
# Installs the .NET 9 SDK, the MAUI Android workload and (best-effort) the Android
# SDK, so the ScaleCounter.Core tests AND the MAUI Android APK can be built in a
# web session. Idempotent and safe to re-run; the container state is cached after
# the hook completes, so the install cost is paid once.
set -euo pipefail

# Only run in the remote (Claude Code on the web) environment.
if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

DOTNET_DIR="$HOME/.dotnet"
ANDROID_DIR="$HOME/android-sdk"
# The Xamarin Android SDK installer requires a user name; the container may not set one.
export USER="${USER:-root}"
export LOGNAME="${LOGNAME:-root}"

# 1) .NET 9 SDK (skip if already present).
if ! "$DOTNET_DIR/dotnet" --version >/dev/null 2>&1; then
  echo "[session-start] Installing .NET 9 SDK into $DOTNET_DIR ..."
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --channel 9.0 --install-dir "$DOTNET_DIR"
fi

# 2) Persist environment variables for the rest of the session.
if [ -n "${CLAUDE_ENV_FILE:-}" ]; then
  {
    echo "export DOTNET_ROOT=\"$DOTNET_DIR\""
    echo "export PATH=\"$DOTNET_DIR:\$PATH\""
    echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1"
    echo "export DOTNET_NOLOGO=1"
    echo "export ANDROID_HOME=\"$ANDROID_DIR\""
    echo "export ANDROID_SDK_ROOT=\"$ANDROID_DIR\""
  } >> "$CLAUDE_ENV_FILE"
fi
export DOTNET_ROOT="$DOTNET_DIR"
export PATH="$DOTNET_DIR:$PATH"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export ANDROID_HOME="$ANDROID_DIR"
export ANDROID_SDK_ROOT="$ANDROID_DIR"

echo "[session-start] dotnet $("$DOTNET_DIR/dotnet" --version)"

# 3) Restore the pure-logic core + tests when the projects already exist
#    (they don't on the very first hook run, before the port is created).
if [ -f ScaleCounter.Core.Tests/ScaleCounter.Core.Tests.csproj ]; then
  echo "[session-start] Restoring ScaleCounter.Core.Tests ..."
  dotnet restore ScaleCounter.Core.Tests/ScaleCounter.Core.Tests.csproj || true
fi

# 4) MAUI Android workload. Best-effort so a network/disk hiccup never blocks the
#    session — the core library and its tests still build with just the SDK.
echo "[session-start] Installing maui-android workload (best-effort) ..."
dotnet workload install maui-android --skip-sign-check || \
  echo "[session-start] maui-android workload not installed (best-effort); MAUI build falls back to CI/dev machine."

# 5) Best-effort: provision the Android SDK (platform + build-tools) so the APK can
#    be built in-session too. Skipped if already present. Never fatal.
if [ -f ScaleCounter.Maui/ScaleCounter.Maui.csproj ]; then
  if [ ! -d "$ANDROID_DIR/platforms" ]; then
    echo "[session-start] Provisioning Android SDK into $ANDROID_DIR (best-effort) ..."
    dotnet build ScaleCounter.Maui/ScaleCounter.Maui.csproj \
      -t:InstallAndroidDependencies \
      -p:AndroidSdkDirectory="$ANDROID_DIR" \
      -p:AcceptAndroidSDKLicenses=True \
      ${JAVA_HOME:+-p:JavaSdkDirectory="$JAVA_HOME"} \
      || echo "[session-start] Android SDK not provisioned (best-effort); MAUI build falls back to CI."
  fi
  dotnet restore ScaleCounter.Maui/ScaleCounter.Maui.csproj || true
fi

echo "[session-start] Done."
