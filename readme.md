Note: This project is **not** affiliated with  [TRIPOD website building services](http://www.tripod.lycos.com/). If you are looking for that, [you can find it here](http://www.tripod.lycos.com/).

#What is Tripod?

Tripod is an example of one way to architect .NET apps.

#What is Tripod not?

Tripod is not the be-all end-all .NET app architecture, and does not try or claim to be. It can even be an *inappropriate* architecture depending on your goals.

#Why Tripod?

Tripod was published by me, Dan Ludwig, as a community contribution. [I read a lot of code](http://stackoverflow.com/users/304832/danludwig), and I see a lot of developers running into problems when they base their project architectures on blog posts, .NET books, or worse, [Microsoft's own tutorials](http://www.asp.net/get-started/).

Let's face it, writing code is easy. Anyone can write code, and [everyone should know how to write a little code](http://code.org/). What's hard is writing *good* code.

##What is *good* code?

It depends on who you ask. When you write code for long enough, your experience leads you to build *opinions*. What may be a perfect set of code to a customer or business leader could look like a horrid tangled mess to a software engineer who looks at it for the first time. Does that make it *bad* code? Yes? But it does everything it's supposed to and keeps X number of families' mouths fed. How is that bad?

In the end, a lot of it boils down to *opinion*. Are you a pragmatic developer who isn't afraid to use brute force to get things done and burn down your task queue? Or do you have an aesthetic for code, and find yourself looking for more elegant solutions? If you're like me, you're probably some mix between the two.

##Is Tripod *good* code?

Maybe, maybe not. Like I said, it depends on who you ask. It is definitely no magic bullet. I believe the ASP.NET (server) parts of Tripod are good code, but I'm not sure if I like the HTML or Javascript parts of it. To be honest, this is the first project I ever used AngularJS with. So that part of it could very well be bad code. I don't think I've read enough good AngularJS code to be able to form a strong opinion on that part of it.

Tripod was published as an example of how to organize and structure *server* (ASP.NET) code. Don't look to it for examples of how to architect the parts of applications that execute on the browser. The Tripod architecture is about all of the code that runs on the server.

What about Tripod do I think is good?

1. Tripod is testable. In this project you will find automatable unit tests and integration tests (though no automated user acceptance tests (yet)). You should not be able to write anything in Tripod that isn't unit- or integration-testable.

2. Tripod is fluent. Wherever possible, Tripod tries to write self-documenting code. Code that reads like English is easier to follow and understand. You will not find very many abbreviated artifact names in Tripod.

3. Tripod uses depenency injection and inversion of control to create decoupled, cohesive software components. This helps me to more easily separate business code (what I often refer to as "interesting" code) from infrastructure code (what I often refer to as "boring" code).


Tripod also employs a few patterns which I have found to be very useful:

1. Generic Repository: This is probably not the generic repository you are used to. With Tripod you will never have to create a boring generic repository class. Instead, you will use generic methods defined on one repository class.

2. Command Query Responsibility Segregation: CQRS is at the very heart of Tripod's architecture. Commands and queries are the messaging mechanisms between users and business code. User wants to see data? Query. User wants to affect data? Command. It's that simple.

3. Dependency Inversion Principle. Okay, I know I've already mentioned this one in the previous #3 above, but it is a pivotal part of the Tripod architecture. Even if you are already using an IoC container, you may be interested to see how Tripod uses it.

Finally, Tripod helps me avoid common pitfalls and other frustrating "anti-patterns":

1. Constructor overinjection: As a rule of thumb, classes should never really depend on more than 5 other components to do their jobs. With Tripod, your components should rarely need more than 3 dependencies.

2. Code repetition: I am a firm believer in the idea that DRY sometimes means *Do* Repeat Yourself. If you find a pattern that works, makes sense, and is easy to follow, repeat it! My personal rule of thumb though is to not repeat more than about 3 lines of code at a time. When I find myself repeating at a threshhold a little higher than this, I start to think about refactoring.

3. Tight coupling: This is just the inverse of the other #3's above. Dependency inversion enables us to flip the code dependencies to decouple them from one another. Instead of having 2 software components where one depends on the other, you create a 3rd component (an interface) which the other 2 both depend on.

##OK, so how does Tripod work?

Download it, run it, explore it, and try figure it out for yourself. If you have any questions or concerns, [post them under issues](https://github.com/danludwig/tripod/issues/). I will write more about it as I build upon it over time. I have only put it on GitHub to share it with a couple of people who already know about it, and it is **not finished**. However if you're looking to explore different ideas, why *not* at least have a look at it?

#License

All code in Tripod projects is licensed under the [MIT License](http://opensource.org/licenses/MIT).
