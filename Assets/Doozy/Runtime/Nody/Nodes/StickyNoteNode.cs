// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Nody.Nodes.Internal;

namespace Doozy.Runtime.Nody.Nodes
{
    /// <summary>
    /// Sticky note visible in the graph only in the Unity Editor
    /// </summary>
    [Serializable]
    [NodyMenuPath("Utils", "Sticky Note")]
    public class StickyNoteNode : SimpleNode
    {
        public StickyNoteNode()
        {
            nodeName = "Sticky Note";
            nodeDescription = GetRandomDescription();
        }
        
        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            flowGraph.GoBack(); //how the f#@k did you get there?
        }

        public string GetRandomDescription()
        {
            var list = new List<string>
            {
                "When I wrote this code, only God and I understood what I did.\n\nNow only God knows",
                "How many programmers does it take to change a light bulb?\n\nNone, that's a hardware problem",
                "Copy-and-Paste was programmed by programmers for programmers actually",
                "Always code as if the person who ends up maintaining your code will be a violent psychopath who knows where you live",
                "There are two ways to write error-free programs; only the third works",
                "99 little bugs in the code. 99 little bugs in the code.\n\nTake one down, patch it around.\n\n127 little bugs in the code",
                "Remember that there is no code faster than no code",
                "One man's crappy software is another man's full-time job",
                "No code has zero defects",
                "Deleted code is debugged code",
                "Don't worry if it doesn't work right.\n\nIf everything did, you'd be out of a job",
                "It's not a bug — it's an undocumented feature",
                "Voodoo Programming",
                "It works on my machine",
                "It compiles; ship it",
                "Q: How different are C and C++?\n\nA: 1. \n\nBecause C — C++ = 1",
                "What's the object-oriented way to get wealthy?\n\nInheritance",
                "C++: Where your friends have access to your private members",
                "Why do Java programmers have to wear glasses?\n\nBecause they don't C#",
                "Q: What did the Java code say to the C code?\n\nA: You've got no class",
                "Knock, knock\n\n...\n\nWho's there?\n\n...\n\n\n*very long pause*\n\n\n...\n\nJava",
                "God is real ... unless declared integer",
                "A SQL query goes into a bar,\n\nwalks up to two tables, and asks,\n\n‘Can I join you?",
                "To understand what recursion is, you must first understand recursion",
                "The best thing about a boolean is even if you are wrong, you are only off by a bit",
                "Two bytes meet.\n\nThe first byte asks, ‘Are you ill?'\n\nThe second byte replies, 'No, just feeling a bit off'",
                "There are 10 kinds of people in the world: Those who know binary and those who don't",
                "There are only two hard things in computer science: cache invalidation and naming things",
                "There's no place like 127.0.0.1",
                "I have not failed. I've just found 10,000 ways that won't work",
                "There is an easy way and a hard way.\n\nThe hard part is finding the easy way",
                "Q: Is the glass half-full or half-empty?\n\nA: The glass is twice as big as it needs to be",
                "In theory, there ought to be no difference between theory and practice.\n\nIn practice, there is",
                "Whitespace is never white",
                "When all else fails... reboot",
                "Without requirements or design, programming is the art of adding bugs to an empty text file",
                "Before software can be reusable it first has to be usable",
                "Have you tried turning it off and on again?"
            };
            return $"{(list.Count == 0 ? "Some text" : list[new Random().Next(list.Count)])}...\n";
        }
    }
}
