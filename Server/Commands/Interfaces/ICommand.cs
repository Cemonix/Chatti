using System;
using Server.Commands;

namespace Server.Interfaces;

public interface ICommand
{
    CommandName Name { get; }
    string Description { get; }
    string Usage { get; }
    string? Execute(string[] parameters);
}
