var mintLib = {
    MintInternal: function(cmIdPtr){
        const cmId = Pointer_stringify(cmIdPtr);
        
        window.unityConfig.mintNft(cmId);
    },
    
    RequestCandyMachineDataInternal: function(cmIdPtr) {
        const cmId = Pointer_stringify(cmIdPtr);

        window.unityConfig.getCandyData(cmId);
    },
}


mergeInto(LibraryManager.library, mintLib);
