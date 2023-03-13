using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;

namespace Shellnet {

	public interface IDrawingEntity {
		event System.EventHandler OnDispose;
	}

	class ItemContainer {
		//public readonly ArrayList Components = new ArrayList();
		readonly ArrayList items = new ArrayList();
		readonly Hashtable entities = new Hashtable();
		public IList Items {
			get {
				return items;
			}
		}
		public virtual Vector GlobalCenter {
			get {
				return Vector.Zero;
			}
		}
		public DrawingItem GetItemFromEntity(IDrawingEntity entity) {
			if(entity==null) return null;
			if(entities.ContainsKey(entity)) {
				return (DrawingItem)entities[entity];
			} else {
				foreach(ItemContainer item in items) {
					DrawingItem i = item.GetItemFromEntity(entity);
					if(i!=null) return i;
				}
				return null;
			}
		}
		public virtual void AddItem(DrawingItem item) {
			lock(this) {
				entities.Add(item.Entity,item);
				item.Parent = this;
				items.Add(item);
			}
		}
		public virtual void RemoveItem(DrawingItem item) {
			lock(this) {
				item.Parent = null;
				items.Remove(item);
				entities.Remove(item.Entity);
			}
		}
		public bool ContainsEntity(object entity) {
			lock(this) return entities.ContainsKey(entity);
		}
		public virtual void BeginLayout() {
			foreach(DrawingItem item in items) {
				item.BeginLayout();
			}
		}
		public virtual void ForceFeedback() {
			foreach(DrawingItem item in items) {
				item.ForceFeedback();
			}
		}
		public virtual void Perform() {
			foreach(DrawingItem item in items) {
				item.Perform();
			}
		}
		public virtual void Layout() {
			foreach(DrawingItem item in items) {
				item.Layout();
			}
			for(int i=0; i<items.Count; ++i) {
				DrawingItem item1 = (DrawingItem)items[i];
				for(int k=i+1; k<items.Count; ++k) {
					DrawingItem item2 = (DrawingItem)items[k];
					if(item2 is LineItem) continue;
					if(item1.OnBorders!=item2.OnBorders) continue;
					Vector dp = item2.LocalCenter-item1.LocalCenter;
					double dl = dp.GetLength()
						-item1.LocalBoundingBox.Size.GetLength()/2
						-item2.LocalBoundingBox.Size.GetLength()/2;
					if(dl<0) dl=0;
					if(dp==Vector.Zero) {
						item2.Move(Vector.Random()*0.01);
					} else {
						double f = Math.Min(4,3/(dl+0.01));
						dp.Normalize();
						item1.v -= dp*f;
						item2.v += dp*f;
					}
				}
			}
		}
		public virtual void EndLayout() {
			foreach(DrawingItem item in items) {
				item.EndLayout();
			}
		}
	}

	class ViewWorld : ItemContainer {
		public ViewWorld() {
		}
		public Snapshot GetSnapshot() {
			Snapshot snapshot = new Snapshot();
			snapshot.AddRange(Items);
			for(int i=0; i<snapshot.Count; ++i) {
				snapshot.AddRange(snapshot[i].Items);
			}
			return snapshot;
		}
	}

	class Snapshot : CollectionBase {
		protected override void OnValidate(object value) {
			if(value is DrawingItem) return;
			throw new InvalidCastException("argument is not drawingitem.");
		}
		public object SyncRoot {
			get {
				return this;
			}
		}
		public DrawingItem this[int index] {
			get {
				return (DrawingItem)base.List[index];
			}
		}
		public void AddRange(ICollection c) {
			base.InnerList.AddRange(c);
		}
		public DrawingItem GetItemAt(Vector point) {
			lock(this.SyncRoot) {
				for(int i=Count-1; i>=0; --i) {
					DrawingItem item = this[i];
					if(!(item is LineItem)) continue;
					if(item.HitTest(point)) return item;
				}
				for(int i=Count-1; i>=0; --i) {
					DrawingItem item = this[i];
					if(item.HitTest(point)) return item;
				}
			}
			return null;
		}
	}

	abstract class CoordinateConverter {
		public abstract Vector ConvertPosition(Vector p);
		public abstract double ConvertDistance(double l);
		public Vector ConvertDistance(Vector d) {
			return new Vector(ConvertDistance(d.x),ConvertDistance(d.y));
		}
		public Boundary Convert(Boundary boundary) {
			return Convert(boundary.Location,boundary.Size);
		}
		public Boundary Convert(Vector location, Vector size) {
			return new Boundary(ConvertPosition(location),ConvertDistance(size));
		}
	}

	abstract class DrawingItem : ItemContainer {
		public readonly StringCollection Labels = new StringCollection();
		public readonly IDrawingEntity Entity = null;
		public ItemContainer Parent = null;
		public bool Selected = false;
		public bool Focused = false;
		public bool OnBorders = false;
		internal Vector v = Vector.Zero;
		public DrawingItem(IDrawingEntity entity) {
			if(entity==null) throw new ArgumentNullException();
			Entity = entity;
			Entity.OnDispose += new EventHandler(Entity_OnDispose);
		}
		public override void AddItem(DrawingItem item) {
			base.AddItem(item);
		}
		public override void RemoveItem(DrawingItem item) {
			base.RemoveItem(item);
		}
		public override void BeginLayout() {
			v = LocalCenter;
			if(v.GetLength2()>1) v.Normalize();
			v *= -2;
			base.BeginLayout();
		}
		public override void ForceFeedback() {
			if(Items.Count==0) return;
			/*
			Vector v0 = Vector.Zero;
			foreach(DrawingItem item in Items) {
				item.ForceFeedback();
				v0 += item.v;
			}
			v0 /= Items.Count;
			this.v += v0;
			foreach(DrawingItem item in Items) {
				item.v -= v0;
			}
			*/
		}
		public override void EndLayout() {
			base.EndLayout();
			this.Move(this.v*0.1+this.LocalCenter);
		}
		public virtual bool Available {get{return true;}}
		public override Vector GlobalCenter {
			get {
				if(Parent==null) return LocalCenter;
				return Parent.GlobalCenter+LocalCenter;
			}
		}
		public abstract Vector LocalCenter {get;}
		public virtual Boundary GlobalBoundingBox {
			get {
				if(Parent==null) return LocalBoundingBox;
				return LocalBoundingBox.Offset(Parent.GlobalCenter);
			}
		}
		public abstract Boundary LocalBoundingBox {get;}
		public abstract void Move(Vector point);
		public abstract bool HitTest(Vector point);
		public abstract void Draw(Graphics g, CoordinateConverter conv);
		public abstract void DrawText(Graphics g, CoordinateConverter conv);

		private void Entity_OnDispose(object sender, EventArgs e) {
			if(this.Parent!=null) this.Parent.RemoveItem(this);
		}
	}

	class LineItem : DrawingItem {
		public Pen ForegroundPen = Pens.LightGray;
		public Brush ForegroundBrush = Brushes.LightGray;
		public DrawingItem Item1;
		public DrawingItem Item2;
		public LineItem(IDrawingEntity entity) : base(entity) {
		}
		public override bool Available {
			get {
				return Item1!=null && Item2!=null;
			}
		}
		public override Vector LocalCenter {
			get {
				return this.GlobalBoundingBox.Center;
			}
		}
		public override Boundary GlobalBoundingBox {
			get {
				return LocalBoundingBox;
			}
		}
		public override Boundary LocalBoundingBox {
			get {
				Vector p1 = Item1.GlobalCenter;
				Vector p2 = Item2.GlobalCenter;
				Boundary b;
				b.Location.x = Math.Min(p1.x,p2.x);
				b.Location.y = Math.Min(p1.y,p2.y);
				b.Size.x = Math.Abs(p2.x-p1.x);
				b.Size.y = Math.Abs(p2.y-p1.y);
				return b;
			}
		}
		public override void Move(Vector offset) {
			//NOP
		}
		public override bool HitTest(Vector point) {
			return (point-this.GlobalBoundingBox.Center).GetLength()<1;
		}
		public override void Perform() {
			base.Perform();
			if(this.Available) {
				Vector dp = Item2.GlobalCenter-Item1.GlobalCenter;
				double f = 0.02;//Math.Max(2,dp.GetLength());
				dp.Normalize();
				dp *= f;
				Item1.v += dp;
				Item2.v -= dp;
			}
		}
		public override void Draw(Graphics g, CoordinateConverter conv) {
			if(this.Available) {
				g.DrawLine(Focused ? Pens.Red : Selected ? Pens.Magenta : ForegroundPen
					,conv.ConvertPosition(Item1.GlobalCenter)
					,conv.ConvertPosition(Item2.GlobalCenter));
				Boundary bd = this.GlobalBoundingBox;
				Vector pt = this.GlobalBoundingBox.Center;
				Rectangle rc = new Rectangle(conv.ConvertPosition(pt), new Size(5,5));
				rc.X -= 2;
				rc.Y -= 2;
				g.FillEllipse(Selected ? Brushes.Red : ForegroundBrush, rc);
			}
		}
		public override void DrawText(Graphics g, CoordinateConverter conv) {
			
		}

	}

	class CircleItem : DrawingItem {
		public Vector Location = Vector.Zero;
		public double Radius = 1;
		public CircleItem(IDrawingEntity entity) : base(entity) {
		}
		public override Vector LocalCenter {
			get {
				return Location;
			}
		}
		public override Boundary LocalBoundingBox {
			get {
				Boundary b;
				b.Location = Location;
				b.Location.x -= Radius;
				b.Location.y -= Radius;
				b.Size.x = Radius*2;
				b.Size.y = Radius*2;
				return b;
			}
		}
		public override void Move(Vector point) {
			Location = point;
		}
		public override bool HitTest(Vector point) {
			return (point-GlobalCenter).GetLength()<=Radius;
		}
		public override void EndLayout() {
			double r0 = 1;
			Vector p0 = Vector.MaxValue;
			Vector p1 = Vector.MinValue;
			int c = 0;
			foreach(DrawingItem item in Items) {
				if(item.OnBorders) continue;
				++c;
				Boundary b = item.LocalBoundingBox;
				double d = b.Center.GetLength()+b.Size.GetLength()/2;
				if(d+0.5>r0) r0=d+0.5;
			}
			if(c>0) Radius += (r0-Radius)*0.5;
			base.EndLayout();
			foreach(DrawingItem item in Items) {
				if(!item.OnBorders) continue;
				Vector dp = item.LocalCenter;
				if(dp.GetLength2()==0) dp.x+=1;
				dp.Normalize();
				item.Move(dp*Radius);
			}
		}
		public Brush Foreground = Brushes.LightBlue;
		public override void Draw(Graphics g, CoordinateConverter conv) {
			Rectangle rc = conv.Convert(GlobalBoundingBox);
			g.FillEllipse(Selected ? Brushes.LightPink : Foreground,rc);
			g.DrawEllipse(Focused ? Pens.Red : Pens.Blue,rc);
		}
		public override void DrawText(Graphics g, CoordinateConverter conv) {
			string text = "";
			foreach(string label in Labels) {
				text += label+"\r\n";
			}
			text = text.TrimEnd('\r','\n');
			using(Font font = new Font(FontFamily.GenericMonospace,10)) {
				SizeF sz = g.MeasureString(text,font);
				PointF pt = conv.ConvertPosition(this.GlobalCenter);
				pt.X -= sz.Width/2;
				pt.Y -= sz.Height/2;
				g.DrawString(text,font,Brushes.Gray,pt);
			}
		}
	}

	class RectItem : DrawingItem {
		public Boundary Boundary = new Boundary(Vector.Zero, new Vector(1,1));
		public RectItem(IDrawingEntity entity) : base(entity) {
		}
		public override Vector LocalCenter {
			get {
				return Boundary.Center;
			}
		}
		public override Boundary LocalBoundingBox {
			get {
				return Boundary;
			}
		}
		public override void Move(Vector point) {
			Boundary.Location = point;
		}
		public override bool HitTest(Vector point) {
			return GlobalBoundingBox.Contains(point);
		}
		public override void Draw(Graphics g, CoordinateConverter conv) {
			Rectangle rc = conv.Convert(GlobalBoundingBox);
			g.FillRectangle(Brushes.LightGreen,rc);
			g.DrawRectangle(Pens.DarkGreen,rc);
		}
		public override void DrawText(Graphics g, CoordinateConverter conv) {
		}
	}

}
