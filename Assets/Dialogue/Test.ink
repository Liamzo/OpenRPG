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
    +   { slay_the_bandits_stage == -1} [Anything I can help with?]
        We need help killing some bandits nearby
            + +     [I'd be happy to help]
                Let me mark the location on your map
                ~slay_the_bandits_stage = 0
                ~startQuest("Slay the Bandits")
                -> opts
            + +     [No thanks]
                -> opts
    *   { slay_the_bandits_stage == 1 } [I've slain the bandits]
        You have my thanks. Here, please take this as a reward
        If you're ever out in the woods again, feel free to kill more of the bastards
        ~slay_the_bandits_stage = 2
        ~dialogueChoiceMade("Slay the Bandits", "Return to Bob", 0)
        -> opts
    *   { slay_the_bandits_stage >= 2 } [Any other jobs for me?]
        Now that you mention it, we could use some more equipment to help defend ourselves
        If you could bring us 5 swords, that would help a lot
            + +     [Sure, I can do that]
                Cheers mate. You'll be sure to find some on the bandits out in the woods
                ~gather_swords_stage = 0
                ~startQuest("Gather Swords")
                -> opts
            + +     [Sounds like a hassle]
                -> opts
                
    *   [Tell me about this place]
        There's not much to say really
        -> opts
    +   [Good bye]
        -> done
                

=== done ===
See you around
-> END