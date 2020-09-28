# fastJSON


Smallest, fastest polymorphic JSON serializer

see the article here : [http://www.codeproject.com/Articles/159450/fastJSON] (http://www.codeproject.com/Articles/159450/fastJSON)

Also see [Howto.md](Howto.md)

## Security Warning

It has come to my attention from the *HP Enterprise Security Group* that using the `$type` extension has the potential to be unsafe, so use it with **common sense** and known json sources and not public facing ones to be safe.

## Security Warning Update
I have added `JSONParameters.BadListTypeChecking` which defaults to `true` to check for known `$type` attack vectors from the paper published from *HP Enterprise Security Group*, when enabled it will throw an exception and stop processing the json. 
