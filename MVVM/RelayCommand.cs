using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Windows.Input;

namespace TrendViewer.MVVM
{
  // The following code is inspired by the work of Josh Smith
  // http://joshsmithonwpf.wordpress.com/
  
  /// <summary>
  /// A command whose sole purpose is to relay its functionality to other
  /// objects by invoking delegates. The default return value for the CanExecute
  /// method is 'true'.
  /// </summary>
  [ExcludeFromCodeCoverage]
  public class RelayCommand : ICommand
  {
    #region private fields
    private readonly Expression<Action> execute;
    private readonly Expression<Action<Object>> executeWithParameter;
    private readonly Func<bool> canExecute;
    private readonly List<EventHandler> canExecuteSubscribers = new List<EventHandler>();

    #endregion

    #region public fields
    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
      // wire the CanExecutedChanged event only if the canExecute func
      // is defined (that improves perf when canExecute is not used)
      add
      {
        if (this.canExecute != null)
        {
          CommandManager.RequerySuggested += value;
          canExecuteSubscribers.Add(value);
        }
      }
      remove
      {
        if (this.canExecute != null)
        {
          CommandManager.RequerySuggested -= value;
          canExecuteSubscribers.Remove(value);
        }
      }
    }
    #endregion

    #region public methods
    /// <summary>
    /// Initializes a new instance of the RelayCommand class
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    public RelayCommand(Expression<Action> execute)
      : this(execute, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RelayCommand class
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Expression<Action> execute, Func<bool> canExecute)
    {
      if (execute == null)
      {
        throw new ArgumentNullException("execute");
      }

      this.execute = execute;
      this.canExecute = canExecute;
    }

    /// <summary>
    /// Initializes command with parameterized delegate
    /// </summary>
    /// <param name="execute">action delegate</param>
    public RelayCommand(Expression<Action<Object>> execute)
      : this(execute, null)
    {
    }

    /// <summary>
    /// Initializes command with parameterized delegate and predicate
    /// </summary>
    /// <param name="execute">action delegate</param>
    /// <param name="canExecute">predicate to check action possibility</param>
    public RelayCommand(Expression<Action<Object>> execute, Func<bool> canExecute)
    {
      if (execute == null)
      {
        throw new ArgumentNullException("execute");
      }

      this.executeWithParameter = execute;
      this.canExecute = canExecute;
    }

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    public void Execute(object parameter)
    {
      if (parameter == null)
      {
        //If execute is null, make sure the signature of the execute method 
        //does not expect a parameter. This will lead to execute being null.
        var methodCallExp = execute.Body as MethodCallExpression;
        if (methodCallExp != null) 
        {
          string methodName = methodCallExp.Method.Name;
        }
        Action action = execute.Compile();
        action();
      }
      else
      {
        var methodCallExp = executeWithParameter.Body as MethodCallExpression;
        if (methodCallExp != null) 
        {
          string methodName = methodCallExp.Method.Name;
        }
        Action<object> action = executeWithParameter.Compile();
        action(parameter);
      }
    }

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
    /// <returns>
    /// true if this command can be executed; otherwise, false.
    /// </returns>
    public bool CanExecute(object parameter)
    {
      return this.canExecute == null || this.canExecute();
    }

    /// <summary>
    /// Ask the UI to reevaluate CanExecute
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
      CommandManager.InvalidateRequerySuggested();
    }
    #endregion
  }
}