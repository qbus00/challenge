# challenge

Some remarks:

* Because of github API limits (especially on pull requests endpoint) too many search and/or pull to refresh actions can cause "Forbidden" responses. 
* I wanted to avoid N+1 queries so there's no first and lastname of an user on search screen.
* GitHub does not provide information about open and total pull requests for repository (or at least I haven't found such one).
Because of that and because of incremental loading there's no gray bar with open/total pull requests on pull requests view.
* Some classes (especially ViewModels) can be refactored (e.g. base classes can be introduced to reduce code duplication) - not done because of lack of time.
* Also because of lack of time I have not implemented Unit/UI Tests.
