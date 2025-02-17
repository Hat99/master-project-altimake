using System;
using System.Collections.Generic;


/**********************/
/*altimate data object*/
/**********************/
[Serializable]
public class Altimate
{
    [Serializable]
    public class Part
    {
        [Serializable]
        public class ImageData: IEquatable<ImageData>, IComparable<ImageData>
        {
            //source path of the image
            public string source = "";
            //"layer" the image is rendered on (higher numbers get rendered in front of lower numbers)
            public float layer = 0.0f;

            //wether or not the image is currently active
            public bool isActive;

            #region cmp
            // Default comparer for ImageData type.
            public int CompareTo(ImageData compareData)
            {
                // A null value means that this object is greater.
                if (compareData == null)
                    return 1;

                else
                    return this.layer.CompareTo(compareData.layer);
            }
            public override int GetHashCode()
            {
                return (source + layer).GetHashCode();
            }
            //image data is "equal" for list purposes if both share the same layer position (they're interchangable)
            public bool Equals(ImageData other)
            {
                if (other == null) return false;
                return (this.layer == other.layer);
            }
            #endregion cmp
        }
        //what the option is supposed to be called (in the menu)
        //in case of alternating option, this is the name of the drop down itself, not it's content
        public string displayName = "";

        //the sub options of this option (for drop downs)
        //sub options with sub options aren't supported for now, but could be an option one day
        public List<Part> subParts = new List<Part>();

        //the image(s) the option should be concerned with
        //drop down options should define the relevant image(s) in the subParts
        public List<ImageData> images = new List<ImageData>();

        //wether or not the option is enabled by default
        public bool onByDefault = true;

        //wether or not the part is an option
        //non-optional binary options equal default parts of the image (e.g. background)
        //optional binary options are toggles
        //non-optional drop down options only allow the selection of each of the sub options
        //optional drop down options also generate an "Off" option
        public bool optional = false;
    }

    public string name = "New Altimate";

    //Which version of Altimate this was made for
    public string altimate_version = "Version 0.1";

    public List<Part> parts = new List<Part>();
}
