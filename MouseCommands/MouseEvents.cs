using System;
using System.Runtime.InteropServices;

namespace MouseCommands
{
    [Flags]
    internal enum MouseEventFlags
    {
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        Move = 0x0001,
        Absolute = 0x8000
    }
    public enum MouseCommButton
    {
        Left = 0x0002,
        Right = 0x0008,
        Middle = 0x0020
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MousePoint
    {
        public int X;
        public int Y;

        public MousePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public enum MouseActionEvent
    {
        Clicked,
        DblClicked,
        MultiClicked,
        Moved,
        Dragged
    }
    public class MouseCommand
    {
        public delegate void MouseActionEventHandler(MouseActionEvent result, MousePoint pos);
        /// <summary>Occurs when Hotkey is Registered.</summary>
        public event MouseActionEventHandler MouseActionEventCall;

        public MousePoint CurrentMousePoint { get {  return GetCursorPosition(); } }

        public MouseCommand()
        {
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }
        private void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        private MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint = new MousePoint(0, 0);
            GetCursorPos(out currentMousePoint);
            return currentMousePoint;
        }
        public MouseCommand _Click(MouseCommButton button = MouseCommButton.Left, int count = 1)
        {
            MousePoint position = CurrentMousePoint;
            return _Click(button, count, position);
        }
        public MouseCommand _Click(MouseCommButton button, int count, MousePoint position)
        {
            MouseEventFlags clk;
            if (button == MouseCommButton.Middle) { clk = MouseEventFlags.MiddleDown | MouseEventFlags.MiddleUp; }
            else if (button == MouseCommButton.Right) { clk = MouseEventFlags.RightDown | MouseEventFlags.RightUp; }
            else { clk = MouseEventFlags.LeftDown | MouseEventFlags.LeftUp; }

            MouseCommand m = Click((int)clk, count, position);

            if (count == 1) { MouseActionEventCall?.Invoke(MouseActionEvent.Clicked, position); }
            else if (count == 2) { MouseActionEventCall?.Invoke(MouseActionEvent.DblClicked, position); }
            else { MouseActionEventCall?.Invoke(MouseActionEvent.MultiClicked, position); }
            return m;
        }

        private MouseCommand Click(int act, int count, MousePoint position)
        {
            for (int i = 1; i <= count; i++)
            { mouse_event((int)act, position.X, position.Y, 0, 0); }
            return this;
        }

        public MouseCommand _DoubleClick(MouseCommButton button = MouseCommButton.Left)
        {
            MousePoint position = CurrentMousePoint;
            return _DoubleClick(button, position);
        }
        public MouseCommand _DoubleClick(MouseCommButton button, MousePoint position)
        {
            return _Click(button, 2, position);
        }


        public MouseCommand _Drag(MousePoint position1, MousePoint position2, MouseCommButton button = MouseCommButton.Left, int speed = 50)
        {
            MouseEventFlags up;
            if (button == MouseCommButton.Middle) { up = MouseEventFlags.MiddleUp; }
            else if (button == MouseCommButton.Right) { up = MouseEventFlags.RightUp; }
            else { up = MouseEventFlags.LeftUp; }

            mouse_event((int)button, position1.X, position1.Y, 0, 0);
            System.Threading.Tasks.Task.Delay(1).Wait();

            if (speed != 0)
            {
                decimal x1 = (decimal)position1.X;
                decimal y1 = (decimal)position1.Y;
                decimal x2 = (decimal)position2.X;
                decimal y2 = (decimal)position2.Y;
                decimal xDif = x1 - x2;
                decimal yDif = y1 - y2;
                decimal avg = (xDif + yDif) / 2;
                decimal steps = (avg / speed);


                //double the steps for smoother transition
                decimal xStep = ((position1.X - position2.X) / steps) / 2; if (position1.X > position2.X) { xStep = -xStep; }
                decimal yStep = ((position1.Y - position2.Y) / steps) / 2; if (position1.Y > position2.Y) { yStep = -yStep; }
                if (steps < 0) { steps = 1; }
                for (int i = 0; i <= steps * 2; i++)
                { SetCursorPosition((int)x1, (int)y1); x1 += xStep; y1 += yStep; System.Threading.Tasks.Task.Delay(1).Wait(); }
            }
            SetCursorPosition(position2);
            System.Threading.Tasks.Task.Delay(1).Wait();
            mouse_event((int)up, position2.X, position2.Y, 0, 0);

            MouseActionEventCall?.Invoke(MouseActionEvent.Dragged, position2);
            return this;
        }
        public MouseCommand _MoveCursorPosition(MousePoint position2, int speed = 50)
        {
            MousePoint position1 = CurrentMousePoint;
            if (speed != 0)
            {
                decimal x1 = (decimal)position1.X;
                decimal y1 = (decimal)position1.Y;
                decimal x2 = (decimal)position2.X;
                decimal y2 = (decimal)position2.Y;
                decimal xDif = x1 - x2;
                decimal yDif = y1 - y2;
                decimal avg = (xDif + yDif) / 2;
                decimal steps = (avg / speed);


                //double the steps for smoother transition
                decimal xStep = ((position1.X - position2.X) / steps) / 2; if (position1.X > position2.X) { xStep = -xStep; }
                decimal yStep = ((position1.Y - position2.Y) / steps) / 2; if (position1.Y > position2.Y) { yStep = -yStep; }
                if (steps < 0) { steps = 1; }
                for (int i = 0; i <= steps * 2; i++)
                { SetCursorPosition((int)x1, (int)y1); x1 += xStep; y1 += yStep; System.Threading.Tasks.Task.Delay(1).Wait(); }
            }
            SetCursorPosition(position2);
            MouseActionEventCall?.Invoke(MouseActionEvent.Moved, position2);
            return this;
        }
    }
}
