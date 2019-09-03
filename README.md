# MouseCommands
Resource to add mouse shortcuts to your .Net projects.
________________________________________________________________

- Reference MouseCommands.dll in your project.
- Create new instance of MouseCommands.
- Set Action MouseActionEventCall (Returns MouseActionEvent and MousePoint)
________________________________________________________________

Setup Example:

  using MouseCommands;
  using System;

  namespace YourProgram
  {
      static class Program
      {
        [STAThread]
        static void Main()
        {
          MouseCommand mouse = new MouseCommand();
          mouse.MouseActionEventCall += OnMouseActionEventCall;
          mouse._MoveCursorPosition(new MousePoint(10, 10), 50);
          mouse._DoubleClick();
        }

        private void OnMouseActionEventCall(MouseActionEvent result, MousePoint pos)
        {
            //do something on Mouse _Actions.
        }
      }
  }

Other Options:
	
	//Returns a MousePoint type with X and Y locations of the mouse.
	mouse.SetHotkeysGlobally = true;
	
	//Default single left click at current mouse position.
  //Can set Left, Middle, or Right clicks with the Click count and mouse position.
  mouse._Click();
  
  //Chaining commands is allowed.
  Mouse._Click()._Click(); //etc...
  
  //Default double left click at current mouse position.
  //Can set Left, Middle, or Right clicks and mouse position.
  mouse._DoubleClick();
  
  //Specify a start position and end position with MousePoint types. Default click is Left.
  //Can set Left, Middle, or Right clicks and the speed to move the mouse. (Higher is faster, 50 = default, speed = 0 is instant).
  mouse._Drag();
  
  //Specify a start position and end position with MousePoint types.
  //Can set the speed to move the mouse. (Higher is faster, 50 = default, speed = 0 is instant).
  mouse._MoveCursorPosition();
	
	mouse.MouseActionEventCall is an event fired when a mouse event is fired from this resource.
  It returns the type of action using the MouseActionEvent type, and the final mouse position in the MousePoint type.
