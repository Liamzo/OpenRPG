INCLUDE globals.ink
VAR already_spoken = false

{already_spoken == false: -> intro | -> intro_known}

=== intro ===
Hello there, it's nice to meet you
~already_spoken = true
    + [It's nice to meet you to]
        -> main
    + [Good day]
        -> done
    


=== intro_known ===
It's nice to see you again
-> main

=== main ===
What can I do for you?
- (opts)
    +   { slay_the_orcs_stage == -1} [Anything I can help with?]
        We need help killing some orcs nearby
            + +     [I'd be happy to help]
                ~slay_the_orcs_stage = 0
                ~startQuest("Slay the Orcs")
                -> opts
            + +     [No thanks]
                -> opts
    *   { slay_the_orcs_stage == 1 } [I've slain the orcs]
        You have my thanks. Here, please take this as a reward
        ~slay_the_orcs_stage = 2
        ~dialogueChoiceMade("Slay the Orcs", "Return to Bob", 0)
        -> opts
    *   [Tell me about this place]
        There's not much to say really
        -> opts
    +   [Good bye]
        -> done
                

=== done ===
See you around
-> END