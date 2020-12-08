using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace AdventOfCode.Y2020.D08 
{
  public class Solution {

    public record RunResult (
       int Idx,
       int Acc,
       bool Complete,
       List<int> Seen
    );

    public string GetName() => "Handheld Halting";
    private string Input = "2020/Day08/input.in";

    public long PartOne() => SolvePartOne(System.IO.File.ReadAllText(Input));
    public long PartTwo() => SolvePartTwo(System.IO.File.ReadAllText(Input));

    long SolvePartOne(string input) {
      var instructions = Instructions(input).ToList();
      return RunInstructions(instructions).Acc;
    }

    bool IsJmpOrNop((string, int) instruction)  {
      var (op, value) = instruction;
      return op is "jmp" or "nop";
    }

    long SolvePartTwo(string input) {
      var instructions = Instructions(input).ToList();
      var result = RunInstructions(instructions);
      var initialResult = result;
      var lastOpIdx  = 0;
      var seenIdx = 0;
      while (!result.Complete) {
        if (result.Complete) return result.Acc;
        for (var i = seenIdx; i < initialResult.Seen.Count; i++)  {
          if (IsJmpOrNop(instructions[initialResult.Seen[i]])) {
            instructions[initialResult.Seen[i]] = FlipOp(instructions[initialResult.Seen[i]]);
            lastOpIdx = i; 
            seenIdx = i + 1;
            break;
          }
        }
        result = RunInstructions(instructions);
        if (!result.Complete) {
          instructions[initialResult.Seen[lastOpIdx]] = FlipOp(instructions[initialResult.Seen[lastOpIdx]]);
        }
      }
      return result.Acc;
    }

    (string, int) FlipOp((string, int) instruction) {
      var (op, value) = instruction;
      if (op == "jmp") {
        return ("nop", value);
      }
      else if (op == "nop") {
        return ("jmp", value);
      }
      else {
        return (op, value);
      }
    }

    RunResult RunInstructions(IList<(string, int)> instructions) {
      var stack = new Stack<(string, int)>();
      stack.Push(instructions.FirstOrDefault());
      var idx = 0;
      var acc = 0;
      var seen = new List<int>();
      var complete = false;
      while (stack.Count > 0) {
        var (op, value) = stack.Pop();
        if (seen.Contains(idx)) {
          break;
        }
        seen.Add(idx);
        switch(op) {
          case "acc":
            acc = acc + value;
            idx++;
            break;
          case "jmp":
            idx = idx + value;
            break;
          case "nop":
            idx++;
            break;
        }
        if (idx < instructions.Count) {
          stack.Push(instructions[idx]);
        }
        else {
          complete = true;
        }
      }
      return new RunResult(idx, acc, complete, seen);
    }

    

    IEnumerable<(string, int)> Instructions(string input) {
      return (
        from line in input.Split("\n")
          let parts = line.Split(' ')
          let instruction = (parts[0], Int32.Parse(parts[1].Replace("+", "")))
        select instruction
      );
    }
    
  }


}