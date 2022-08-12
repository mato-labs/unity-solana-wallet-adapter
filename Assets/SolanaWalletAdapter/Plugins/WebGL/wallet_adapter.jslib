var myLib = {
    InitInternal: function () {
        window.unityConfig.createUnityStr = function(str) {
            var bufferSize = lengthBytesUTF8(str) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(str, buffer, bufferSize);
            return buffer;
        };
    },

    DisconnectWalletByNameInternal: function (walletName) {
       const walletId = Pointer_stringify(walletName);
       
       window.unityConfig.disconnectWalletByName(walletId);
    },

    ConnectWalletByNameInternal: function(walletName) {
        const walletId = Pointer_stringify(walletName);

        window.unityConfig.connectWalletByName(walletId);
    },
    
    GetWalletConfigInternal: function(){
      const r = window.unityConfig.getWalletDataJsonStr();
      
      return window.unityConfig.createUnityStr(r);
    },
    
    RequestMintMetadataInternal: function(mint){
        const mintStr = Pointer_stringify(mint);
        
        window.unityConfig.requestMintMetadata(mintStr);
    },
    
    SignMessageInternal: function(walletName, messageStr) {
        const walletId = Pointer_stringify(walletName);
        const signMessage = Pointer_stringify(messageStr);

        window.unityConfig.signMessageWithWalletByName(walletId, signMessage);
    },
    
    SendTransactionInternal: function(walletName, base64tx) {
        const walletId = Pointer_stringify(walletName);
        const base64transaction = Pointer_stringify(base64tx);

        window.unityConfig.sentTransactionWithWalletByName(walletId, base64transaction);
    },
}


mergeInto(LibraryManager.library, myLib);
