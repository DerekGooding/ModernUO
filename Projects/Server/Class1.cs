using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server;

internal class Thing1 : IEntity
{
    public Point3D Location { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Map Map => throw new NotImplementedException();

    public int Z => throw new NotImplementedException();

    public int X => throw new NotImplementedException();

    public int Y => throw new NotImplementedException();

    public DateTime Created { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Serial Serial => throw new NotImplementedException();

    public bool Deleted => throw new NotImplementedException();

    public byte SerializedThread { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int SerializedPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int SerializedLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Delete() => throw new NotImplementedException();
    public void Deserialize(IGenericReader reader) => throw new NotImplementedException();
    public bool InRange(Point2D p, int range) => throw new NotImplementedException();
    public bool InRange(Point3D p, int range) => throw new NotImplementedException();
    public void MoveToWorld(Point3D location, Map map) => throw new NotImplementedException();
    public void OnMovement(Mobile m, Point3D oldLocation) => throw new NotImplementedException();
    public bool OnMoveOff(Mobile m) => throw new NotImplementedException();
    public bool OnMoveOver(Mobile m) => throw new NotImplementedException();
    public void ProcessDelta() => throw new NotImplementedException();
    public void RemoveItem(Item item) => throw new NotImplementedException();
    public void Serialize(IGenericWriter writer) => throw new NotImplementedException();
}
internal class Class1 : ISerializable
{
    public DateTime Created { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Serial Serial => throw new NotImplementedException();

    public bool Deleted => throw new NotImplementedException();

    public byte SerializedThread { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int SerializedPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int SerializedLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Delete() => throw new NotImplementedException();
    public void Deserialize(IGenericReader reader) => throw new NotImplementedException();

    public void Do()
    {
        Thing1 thing = new();
        this.Delete(thing);
    }

    public void Serialize(IGenericWriter writer) => throw new NotImplementedException();
}
