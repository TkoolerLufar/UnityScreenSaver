package jp.tkoolerlufar.madewithunity.screensaver

import android.content.Intent
import android.content.res.Configuration
import android.service.dreams.DreamService
import android.view.KeyEvent
import com.unity3d.player.IUnityPlayerLifecycleEvents
import com.unity3d.player.UnityPlayer

/**
 * A service to provide the made with Unity as a screen saver (also known as a Daydream).
 */
open class UnityPlayerDreamService : DreamService(), KeyEvent.Callback,
    IUnityPlayerLifecycleEvents {
    /**
     * My UnityPlayer instance.
     *
     * It's defined for Unity native code.
     */
    @JvmField @Deprecated("Prevent from using directly from Kotlin code")
    protected var mUnityPlayer: UnityPlayer? = null

    /**
     * My UnityPlayer instance.
     */
    var myUnityPlayer: UnityPlayer
        get() {
            @Suppress("DEPRECATION")
            return this.mUnityPlayer!!
        }
        private set(that) {
            @Suppress("DEPRECATION")
            this.mUnityPlayer = that
        }

    /**
     * Override this in your custom UnityPlayerActivity to tweak the command line arguments passed to the Unity Android Player
     * The command line arguments are passed as a string, separated by spaces
     * UnityPlayerActivity calls this from 'onCreate'
     * Supported: -force-gles20, -force-gles30, -force-gles31, -force-gles31aep, -force-gles32, -force-gles, -force-vulkan
     * See https://docs.unity3d.com/Manual/CommandLineArguments.html
     *
     * @param cmdLine the current command line arguments, may be null
     * @return the modified command line string or null
     */
    protected open fun updateUnityCommandLineArguments(cmdLine: String): String {
        return cmdLine
    }

    /**
     * Set up the screen saver.
     */
    override fun onAttachedToWindow() {
        super.onAttachedToWindow()

        // Attach me UnityPlayerDream
        try {
            UnityPlayerDream.currentService = this
        } catch (_: IllegalStateException) {
            this.finish()
            return
        }

        // 全画面で動かす
        // Run on full screen
        this.isFullscreen = true

        // 画面をタッチで閉じるような動作をUnityのマネージドコードで
        // 実装したいときはこれを true にする
        // Make it true when do I want to implement some features
        // such as Touch To Quit by Unity managed code
        this.isInteractive = true

        // コマンドライン引数を整える
        // Format a command line args
        val cmdLine = this.updateUnityCommandLineArguments("")

        // Unity プレイヤーの起動
        // Start up UnityPlayer
        this.myUnityPlayer = UnityPlayer(this, this).also { unityPlayer ->
            // コマンドライン引数を渡す
            // Give this the command line args
            unityPlayer.newIntent(Intent(Intent.ACTION_DEFAULT).apply {
                putExtra("unity", cmdLine)
            })
            this.setContentView(unityPlayer)
            unityPlayer.requestFocus()
        }
    }

    /**
     * Start to animate the screen saver.
     */
    override fun onDreamingStarted() {
        super.onDreamingStarted()
        this.myUnityPlayer.resume()
    }

    /**
     * Stop animating the screen saver.
     */
    override fun onDreamingStopped() {
        super.onDreamingStopped()
        this.myUnityPlayer.pause()
    }

    /**
     * Tear down the screen saver.
     */
    override fun onDetachedFromWindow() {
        // Detach me UnityPlayerDream
        UnityPlayerDream.currentService = null

        this.myUnityPlayer.destroy()
        super.onDetachedFromWindow()
    }

    override fun onLowMemory() {
        super.onLowMemory()
        this.myUnityPlayer.lowMemory()
    }

    override fun onTrimMemory(level: Int) {
        super.onTrimMemory(level)
        if (level == TRIM_MEMORY_RUNNING_CRITICAL) {
            this.myUnityPlayer.lowMemory()
        }
    }

    override fun onConfigurationChanged(newConfig: Configuration) {
        super.onConfigurationChanged(newConfig)
        this.myUnityPlayer.configurationChanged(newConfig)
    }

    /**
     * Notify Unity of the focus change.
     *
     * @param hasFocus If thw window of the screen saver has focused now.
     */
    override fun onWindowFocusChanged(hasFocus: Boolean) {
        super.onWindowFocusChanged(hasFocus)
        this.myUnityPlayer.windowFocusChanged(hasFocus)
    }

    // For some reason the multiple keyevent type is not supported by the ndk.
    // Force event injection by overriding dispatchKeyEvent().
    override fun dispatchKeyEvent(event: KeyEvent?): Boolean {
        @Suppress("DEPRECATION")
        return if (event!!.action == KeyEvent.ACTION_MULTIPLE) {
            this.myUnityPlayer.injectEvent(event)
        } else {
            super.dispatchKeyEvent(event)
        }
    }

    // Pass any events not handled by (unfocused) views straight to UnityPlayer
    override fun onKeyUp(keyCode: Int, event: KeyEvent?): Boolean {
        return this.myUnityPlayer.onKeyUp(keyCode, event)
    }

    override fun onKeyDown(keyCode: Int, event: KeyEvent?): Boolean {
        return this.myUnityPlayer.onKeyDown(keyCode, event)
    }

    override fun onKeyLongPress(keyCode: Int, event: KeyEvent?): Boolean {
        return this.myUnityPlayer.onKeyLongPress(keyCode, event)
    }

    /**
     * For some reason the multiple keyevent type is not supported by the ndk.
     * Force event injection by overriding dispatchKeyEvent().
     */
    override fun onKeyMultiple(p0: Int, p1: Int, event: KeyEvent?): Boolean {
        return this.myUnityPlayer.injectEvent(event)
    }

    /**
     * When Unity player unloaded stop the screensaver.
     */
    override fun onUnityPlayerUnloaded() {
        this.finish()
    }

    /**
     * When Unity player calls Application.Quit() finish the screensaver.
     */
    override fun onUnityPlayerQuitted() {
    }
}
