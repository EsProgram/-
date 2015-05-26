//https://github.com/interslice/poorsat

using System;
using System.Collections.Generic;
using System.Linq;

namespace Poorsat
{
  /// <summary>
  /// Poor man's implementation of a SAT solving algorithm. Probably get a
  /// C-grade or something if submitted as a university asignment.
  /// </summary>
  public class DefaultSolver
  {
    protected const int Undefined = -1;
    protected const int True = 1;
    protected const int False = 0;

    protected List<List<string>> clauses;
    protected List<string> literals;

    public Dictionary<string, bool> Model { get; protected set; }

    public DefaultSolver()
    {
      clauses = new List<List<string>>();
      literals = new List<string>();
    }

    public DefaultSolver(List<List<string>> clauses)
    {
      this.clauses = clauses;

      literals = new List<string>();
      foreach(var clause in clauses)
      {
        AddClause(clause);
      }
    }

    /// <summary>
    /// Add a new clause to the boolean formula.
    /// </summary>
    /// <param name="clause">The clause to be added</param>
    public void AddClause(List<string> clause)
    {
      foreach(var variable in clause)
      {
        if(!literals.Contains(variable))
        {
          if(variable[0] == '-')
          {
            literals.Add(variable.Substring(1));
          }
          else
          {
            literals.Add(variable);
          }
        }
      }
    }

    /// <summary>
    /// Determines whether a boolean formula, i.e. the current list of
    /// clauses, can be satisfied (also generates a satisfiable variable
    /// configuration).
    /// </summary>
    /// <param name="model">An initial variable configuration to work with</param>
    /// <returns>Whether the boolean formula is satisfiable</returns>
    public bool Solve(Dictionary<string, bool> model = null)
    {
      if(model == null)
      {
        model = new Dictionary<string, bool>();
      }

      // Check if all clauses are satisfiable, i.e. the boolean formula
      // can be satisfied.
      if(clauses.All(clause => Satisfiable(clause, model) == True))
      {
        // Set some public property to the satisfiable variable
        // configuration for external access.
        // TODO: Come up with a better name for the property.
        Model = model;
        return true;
      }

      // Check if a clause is unsatisfiable, i.e. the current variable
      // configuration won't satisfy the boolean formula.
      if(clauses.Any(clause => Satisfiable(clause, model) == False))
      {
        return false;
      }

      // The current variables in our interpretation are insufficient to
      // determine satisfiability. Choose a new variable to add to the
      // variable configuration.
      string choice = null;
      for(var i = 0; i < literals.Count; i++)
      {
        if(!model.ContainsKey(literals[i]))
        {
          choice = literals[i];
          break;
        }
      }

      // If there are no more variables to add then the current
      // interpretation is not satisfiable.
      if(choice == null)
      {
        return false;
      }

      return Solve(Update(model, choice, true)) ||
          Solve(Update(model, choice, false));
    }

    /// <summary>
    /// Determine a new variable configuration given some new literal and
    /// its assignment.
    /// </summary>
    /// <param name="model">The current variable configuration</param>
    /// <param name="choice">The chosen variable to add to the configuration</param>
    /// <param name="value">The assignment for the chosen variable</param>
    /// <returns>The new variable configuration</returns>
    protected Dictionary<string, bool> Update(Dictionary<string, bool> model, string choice, bool value)
    {
      var copy = new Dictionary<string, bool>(model);
      copy[choice] = value;
      return copy;
    }

    /// <summary>
    /// Determines whether a clause can be satisfied given a certain
    /// variable configuration.
    /// </summary>
    /// <param name="clause">The clause to test</param>
    /// <param name="model">The given variable configuration</param>
    /// <returns>Where the given clause is satisfiable</returns>
    protected int Satisfiable(List<string> clause, Dictionary<string, bool> model)
    {
      // All variables in the clause is false, i.e. the clause is
      // unsatisifiable.
      if(clause.All(variable => Resolve(variable, model) == False))
      {
        return False;
      }

      // A variable in the clause is true, i.e. the clause is satisfiable.
      if(clause.Any(variable => Resolve(variable, model) == True))
      {
        return True;
      }

      // We were unable to determine whether the clause was satisfiable.
      return Undefined;
    }

    /// <summary>
    /// Resolve some given variable to its actual value based on a given
    /// variable configuration.
    /// </summary>
    /// <param name="variable">The given variable</param>
    /// <param name="model">The given variable configuration</param>
    /// <returns>The actual value of the given variable</returns>
    protected int Resolve(string variable, Dictionary<string, bool> model)
    {
      if(variable[0] == '-')
      {
        if(model.ContainsKey(variable.Substring(1)))
        {
          return Convert.ToInt32(!model[variable.Substring(1)]);
        }
        else
        {
          return Undefined;
        }
      }
      else
      {
        if(model.ContainsKey(variable))
        {
          return Convert.ToInt32(model[variable]);
        }
        else
        {
          return Undefined;
        }
      }
    }
  }
}
