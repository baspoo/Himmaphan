using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Example-----------------
 * ------------------------------------------------------------------------------------
 * 		this.reuseScrollView.OnGetNumberOfRow = (scrollView) => {
			return MessageDatas.Count; 
		};
		this.reuseScrollView.OnItemForRowAtIndex = (scrollView, item, index) => {
			item.GetComponent<MessageObj>().Init_messagechat(MessageDatas[index]);
		};
		this.reuseScrollView.ReloadData ();
		bool isMoveToDown = (ui_slider.value >= 0.95);
		this.reuseScrollView.ScrollView.UpdateScrollbars ();
		if (MessageDatas.Count >= 9) {
			if (isMoveToDown) {
				ui_slider.value = 1.0f;
			}
		}
 *-----------------------------------------------------------------------------------------
*/



public class ReuseScrollViewDelegate {

}

public class ReuseScrollView : MonoBehaviour {

    public UIScrollView ScrollView;
    public GameObject HeaderAndFooterPrefab;
    public GameObject ItemPrefab;
    public int SizeForRow = 50;

    public UIScrollBar ScrollBar;
    public UIScrollView.ShowCondition showCondition;

    private bool IsReady = false;
    private int NumberOfRow = 1000;
    private float StartPosition = 0;

    private GameObject HeaderItem;
    private GameObject FooterItem;
    private List<GameObject> Items;
    public List<GameObject> ItemsOnRow { get { return Items; } }

    private UIPanel m_panel;
    public UIPanel panel
    {
        get
        {
            if (m_panel == null)
                m_panel = this.ScrollView.gameObject.GetComponent<UIPanel>();
            return m_panel;
        }
    }


    public delegate int GetNumberOfRow(UIScrollView scrollView);
    public GetNumberOfRow OnGetNumberOfRow;

    public delegate void ItemForRowAtIndex(UIScrollView scrollView, GameObject item, int index);
    public ItemForRowAtIndex OnItemForRowAtIndex;

    public delegate void DidSelectItemForRowAtIndex(UIScrollView scrollView, GameObject item, int index);
    public DidSelectItemForRowAtIndex OnDidSelectItemForRowAtIndex;

    public delegate void OnUpdateScrollview();
    public OnUpdateScrollview OnUpdateWhenDragging;
    public OnUpdateScrollview OnUpdateAlways;


    void Start () {

		panel.clipOffset = new Vector2 (0, 0);

    }

	public void ReloadData () {

		if (this.OnGetNumberOfRow != null)
			this.NumberOfRow = this.OnGetNumberOfRow (this.ScrollView);

		if (this.HeaderAndFooterPrefab != null) {
			if (this.HeaderItem == null) {
				this.HeaderItem = Instantiate (this.HeaderAndFooterPrefab, this.ScrollView.transform, true) as GameObject;
				this.HeaderItem.transform.localScale = new Vector3 (1, 1, 1);
			}
			if (this.FooterItem == null) {
				this.FooterItem = Instantiate (this.HeaderAndFooterPrefab, this.ScrollView.transform, true) as GameObject;
				this.FooterItem.transform.localScale = new Vector3 (1, 1, 1);
			}
		}



		if (this.ScrollView.movement == UIScrollView.Movement.Vertical) {
			this.StartPosition = panel.GetViewSize ().y / 2 - SizeForRow / 2;

			if(this.HeaderItem!=null)this.HeaderItem.transform.localPosition = new Vector3 (0, this.StartPosition + SizeForRow / 2, 0);
			if(this.FooterItem!=null)this.FooterItem.transform.localPosition = new Vector3 (0, this.StartPosition - this.SizeForRow * NumberOfRow + SizeForRow / 2, 0);

			if (this.Items == null)
				this.Items = new List<GameObject> ();

			int count = Mathf.CeilToInt (panel.GetViewSize ().y / this.SizeForRow) + 1;
			for (int i = 0; i < count; i++) {
				GameObject item = null;
				if (i < this.Items.Count) {
					item = this.Items [i];
				} else {
					item = Instantiate (this.ItemPrefab, this.ScrollView.transform, true) as GameObject;
					item.transform.localScale = new Vector3 (1, 1, 1);
					item.transform.localPosition = new Vector3 (0, this.StartPosition - this.SizeForRow * i, 0);
					this.Items.Add (item);
				}
			}

		} else {
			this.StartPosition = -panel.GetViewSize ().x / 2 + SizeForRow / 2;

			if(this.HeaderItem!=null)this.HeaderItem.transform.localPosition = new Vector3 (this.StartPosition - SizeForRow / 2 + 2, 0, 0);
			if(this.FooterItem!=null)this.FooterItem.transform.localPosition = new Vector3 (this.StartPosition + this.SizeForRow*NumberOfRow - SizeForRow / 2 - 2, 0, 0);

			if (this.Items == null)
				this.Items = new List<GameObject> ();

			int count = Mathf.CeilToInt (panel.GetViewSize ().x / this.SizeForRow) + 1;
			for (int i = 0; i < count; i++) {
				GameObject item = null;
				if (i < this.Items.Count) {
					item = this.Items [i];
				} else {
					Vector3 postion = new Vector3 (this.StartPosition + this.SizeForRow * i, 0, 0);
					item = Instantiate (this.ItemPrefab , postion , Quaternion.identity) as GameObject;
					item.transform.parent = this.ScrollView.transform;
					item.transform.localScale = Vector3.one;
					item.transform.localPosition = postion;
					this.Items.Add (item);
				}
			}
		}


        // Defualt
        ScrollView.onDragStarted = () => isMoving = true;
        ScrollView.onMomentumMove = () => isMoving = true;
        ScrollView.onStoppedMoving = () => isMoving = false;

        OnUpdateWhenDragging = () => { this.Update(false); };
        OnUpdateAlways = OnUpdateWhenDragging;

        this.IsReady = true;
        this.Update(true);

        InitScrollbar();

    }
    bool isMoving = false;



    public void MoveToValue(float value,bool isForce = false)
    {
        if (ScrollBar == null)
            return;

        if (currentValue != value || isForce)
        {
            ScrollBar.value = value;
            currentValue = ScrollBar.value;
            var index = (int)(value * (float)NumberOfRow);
                MoveToIndex(index);
        }
    }


    public void MoveToIndex ( int index ) {
        if (!IsReady) return;
        int itemPerRow = this.Items.Count;
        if(NumberOfRow > itemPerRow) {
            index = index + itemPerRow > NumberOfRow ? index - itemPerRow + 1 : index;
        }

        UIPanel panel = this.ScrollView.gameObject.GetComponent<UIPanel> ();
        if (this.ScrollView.movement == UIScrollView.Movement.Vertical) 
		{
			float position  = SizeForRow*index;
			panel.clipOffset = new Vector2 (panel.clipOffset.x, -position);
			this.ScrollView.transform.localPosition = new Vector3 (this.ScrollView.transform.localPosition.x, position , 0 );
		} 
		else
		{
			float position  = SizeForRow*index;
			panel.clipOffset = new Vector2 (position,0);
			this.ScrollView.transform.localPosition = new Vector3 ( -position, 0 , 0);
		}

        Update(false, index);
        if (index == 0)
            ScrollView.ResetPosition();
    }

    public void SelectItemAtIndex(int index){
        int i = index % this.Items.Count;
        this.OnDidSelectItemForRowAtIndex(this.ScrollView, this.Items[i], index);
    }

    float currentValue = 0.0f;
    public void ChangeScrollbar() {
        UpdateScrollbar(ScrollBar.value);
    }
    public void UpdateScrollbar(int start, int end)
    {

            float center = (float)(start + end) / 2.0f;
            float value = start == 0 ? 0 : end == NumberOfRow ? 1 : (float)center / (float)NumberOfRow;
            UpdateScrollbar(value);
    }

    public void UpdateScrollbar(float value,bool isForce = false)
    {
        if (ScrollBar != null)
        {
            if (currentValue != value || isForce)
            {
                ScrollBar.value = value;
                currentValue = ScrollBar.value;
                if(isForce)
                    MoveToValue(value,true);
            }
        }
    }
    public void InitScrollbar()
    {
        if (ScrollBar != null)
        {
            ScrollBar.barSize = (float)Items.Count / (float)NumberOfRow;

            if (showCondition == UIScrollView.ShowCondition.WhenDragging)
            {
                var OnUpdate = this.OnUpdateWhenDragging;
                this.OnUpdateWhenDragging = () =>
                {
                    ScrollBar.alpha = 1.0f;
                    OnUpdate();
                };
                this.OnUpdateAlways = () =>
                {
                    ScrollBar.alpha = 0.0f;
                    OnUpdate();
                };

            }
            else
            {

                if (showCondition == UIScrollView.ShowCondition.OnlyIfNeeded)
                {
                    // Hide scroll bar if items less than max row
                    ScrollBar.gameObject.SetActive(NumberOfRow > Items.Count);
                }
                else
                {
                    ScrollBar.gameObject.SetActive(true);
                }

                this.OnUpdateAlways = () =>
                {
                    MoveToValue(ScrollBar.value);
                };

            }
        }

    }

    // Update is called once per frame
    void Update () {
        if (IsReady)
        {
            if (isMoving)
                this.OnUpdateWhenDragging();
            else
                this.OnUpdateAlways();
        }
	}

	void Update (bool force,int forceIndex = -1) {
		if (!IsReady)
			return;

        if (this.ScrollView.movement == UIScrollView.Movement.Vertical) {
            float offset = -panel.clipOffset.y;
           
			int start = forceIndex > -1 ? forceIndex : (int)Mathf.Max (0, (offset / this.SizeForRow));
			int end = (int)Mathf.Min (this.NumberOfRow, start + this.Items.Count);

            UpdateScrollbar(start, end);
            //Debug.LogFormat ("Vertical Range [{0}, {1}]", start, end);
			for (int i = start; i < end; i++) {
				int index = i % this.Items.Count;
				GameObject item = this.Items [index];
				float newPoisition = this.StartPosition - this.SizeForRow * i;
				if (item.transform.localPosition.y != newPoisition || force)
				{
					item.transform.localPosition = new Vector3(0, newPoisition, 0);
					if (this.OnItemForRowAtIndex != null)
					{
						this.OnItemForRowAtIndex(this.ScrollView, item, i);
					}
				}
			}
		} else {
			float offset = panel.clipOffset.x;

			int start = forceIndex > -1 ? forceIndex : (int)Mathf.Max (0, (offset / this.SizeForRow));
			int end = (int)Mathf.Min (this.NumberOfRow, start + this.Items.Count);
            UpdateScrollbar(start, end);
            //Debug.LogFormat ("Range [{0}, {1}]", start, end);
            for (int i = start; i < end; i++) {
				int index = i % this.Items.Count;
				GameObject item = this.Items [index];
				float newPoisition = this.StartPosition + this.SizeForRow * i;
				if (item.transform.localPosition.x != newPoisition || force) {
					item.transform.localPosition = new Vector3 (newPoisition, 0, 0);
					if (this.OnItemForRowAtIndex != null)
						this.OnItemForRowAtIndex (this.ScrollView, item, i);
				}
			}
		}
	}
}
