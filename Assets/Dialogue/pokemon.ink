INCLUDE globals.ink

{already_spoken == false: -> intro | -> have_pokemon}

VAR already_spoken = false
=== intro ===
It's nice to meet you
~already_spoken = true
-> have_pokemon

=== have_pokemon ===
#audio:animal_crossing_low
{ pokemon_name == "": -> main | -> already_chose }

=== main ===
Which pokemon do you choose?
    + [Charmander]
        -> chosen("Charmander")
    + [Bulbasaur]
        -> chosen("Bulbasaur")
    + [Squirtle]
        -> chosen("Squirtle")
        
=== chosen(pokemon) ===
~ pokemon_name = pokemon
You chose {pokemon}!
-> END

=== already_chose ===
You already chose {pokemon_name}!
-> END