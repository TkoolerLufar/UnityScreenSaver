package jp.tkoolerlufar.madewithunity.screensaver

class UnityPlayerDream {
    companion object {
        var currentService: UnityPlayerDreamService? = null
            internal set(dreamService) {
                if (dreamService !== null
                    && field !== null
                    && dreamService !== field
                ) {
                    throw IllegalStateException("Only one UnityPlayer can run at once.")
                }
                field = dreamService
            }
    }
}
