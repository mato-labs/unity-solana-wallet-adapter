using SolanaWalletAdapter.Scripts.Dto;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolanaWalletAdapter.Demo.Scripts
{
    public class NftView: MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _name;

        [SerializeField]
        private Image _icon;
        
        public NftMetadata Metadata { get; private set; }
        
        public void Init(NftMetadata metadata)
        {
            Metadata = metadata;
            
            _name.text = metadata.Name;
            _icon.sprite = metadata.Sprite;
        }
    }
}