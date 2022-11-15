using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//namespace Data 
//{
//    [System.Serializable]
//    public class PlistData
//    {
//        public string version;
       
//        public ApiData apiData;
//        [System.Serializable]
//        public class ApiData
//        {
//            public string api_getEventList = "http://smartobm-vsr-uat.thaitrade.com/api/public/events";
//            public string api_getEventDetail = "http://smartobm-vsr-uat.thaitrade.com/api/public/events/@";
//            public string api_getProductList = "http://smartobm-vsr-uat.thaitrade.com/api/public/sellers/@/products";
//        }

//        public ConfigData config;
//        [System.Serializable]
//        public class ConfigData
//        {
//            public float playerMoveSpeed;
//            public float cameraFieldOfView;
//            public float pitchSensitivity;
//            public float yawSensitivity;
//            public float guiScaleFactor;
//            public int guiWidth;
//            public int guiHeight;
//            public bool protectImageFail;
//        }
//        public Dictionary<string, string> languages = new Dictionary<string, string>();
//    }
//}
namespace Data
{
    [System.Serializable]
    public class EventData
    {
        public string name;
        public int id;
        public object startDate;
        public object endDate;
        //public bool logo;
        public string landingImage;
        //public int floor;
        public List<FloorData> floors;
    }

    [System.Serializable]
    public class FloorData
    {
        public string name;
        public int id;
        public int floorNo;
        public List<BoothData> booths;
    }

    [System.Serializable]
    public class BoothData
    {
        //public string name;
        public int id;
        public int boothNo;
        //public string desc;
        //public int index;
        public string modelId;
        //public string sellerCode;
        //public string coverUrl;
        //public string storeUrl;
        //public string pdfUrl;
        //public string logo;
        public SellerData seller;
      



    }

    [System.Serializable]
    public class SellerData
    {
        public int id;
        public string sellerCode;
        public string description;
        //public string cover;
        public string logo;
        public string eBrochure;
        public CompanyData company;
        public List<BannerData> banners;


        public string GetBanner(Data.BannerData.BannerType type)
        {
            if (banners != null)
            {
                var banner = banners.Find(x => x.type == type.ToString());
                return banner != null ? banner.url : string.Empty;
            }
            else return string.Empty;
        }
        public List<string> GetBanners(Data.BannerData.BannerType type)
        {
            var let = new List<string>();
            if (banners != null)
            {
                foreach (var banner in banners.FindAll(x => x.type == type.ToString()))
                {
                    let.Add(banner.url);
                }
            }
            return let;
        }
    }
    [System.Serializable]
    public class CompanyData
    {
        public string companyName;
        public string storeUrl;
    }


    [System.Serializable]
    public class BannerData
    {
        public enum BannerType
        {
            logo,
            main,
            stand,
            billboard,
            cover
        }
        public enum MediaType
        {
            image,
            video
        }
        public int id;
        public string type;
        public string mediaType;
        public string url;
    }



        [System.Serializable]
    public class ProductData
    {
        public string name;
        public int id;
        public string description;
        //public string thaitradeId;
        //public string internalSku;
        public List<ImageData> images;
        public List<CategoryData> categories;
        public List<InformationData> packagings;
        public List<InformationData> tastes;
        public List<InformationData> styles;
        public List<InformationData> rawMaterials;
        public string previewUrl => (images == null || images.Count == 0) ? string.Empty : images[0].url;
    }

    [System.Serializable]
    public class ImageData
    {
        public string name;
        public int id;
        public string url;
    }
    [System.Serializable]
    public class InformationData
    {
        public string name;
        public int id;
        public string description;
    }
    [System.Serializable]
    public class CategoryData
    {
        public string name;
        public int id;
        public InformationData parentCategory;
    }
}



