### Record types
Record types take a string[] and return an object (either a string, or string[]) to be stored
```coffeescript
para {
    # sets boundaries on the whole document, not on
    #  each paragraph
    include: section{} #optional
    
    # indicates the start of a new paragraph
    start:exp{} #required.
    
    # indicates the end of a paragraph
    stop:exp{} #optional
    
    # the list of propery parsers.
    # you can include any Record type
    props: [ record{} record{} ... ] #required.
}

table {
    header:exp{} #required
}

prop {
    name:"val" #required
    
    line:exp{} #required
    
    # if present, the string[] is combined and 
    #  split again using the provided delimiter
    split:"val" #optional
    
    # the list of parsers to apply to the string
    parsers: [ parser{} parser{} ... ] #required
}

```

### Blocks
```coffeescript

section {
 start: exp{} #optional
 stop: exp{} #optional
}

```

### Parser types
```coffeescript
to {
    # array of strings to search for.
    #  parses to instance of first item 
    #  in array found.
    s:["val"] #required
    
    # i == ignore case
    # w == include search pattern length in index
    # t == trim the returned value
    # l == to last instance of search pattern
    m:"iwtl" #optional
    
    # to specific instance of s value
    q:"2" #optional, if populated, l is ignored
}

after {
    # array of strings to search for.
    #  parses after instance of first item 
    #  in array found.
    s:["val"] #required
    
    # i == ignore case
    # w == include search pattern length in index
    # t == trim the returned value
    # l == after last instance of search pattern
    m:"iwtl" #optional
    
    # after specific instance of s value
    q:"2" #optional, if populated, l is ignored
}

split {
    # split the string on the provided values
    s:["val"] #required
    
    # i == ignore case
    # r == remove empty entries
    # t == trim the returned value
    m:"irt" #optional
    
    # index of item to return
    i:"2"  #required
}

```


### Exp types
```coffeescript
regex {
    s:["val"] #required
    
    # i == ignore case
    # n == negate
    m:"i" #optional
}

contains {
    s:["val"] #required
    
    # i == ignore case
    # n == negate
    m:"ni"  #optional
}

startswith {
    s:["val"] #required
    
    # i == ignore case
    # n == negate
    m:"ni" #optional
}

endswith {
    s:["val"] #required
    
    # i == ignore case
    # n == negate
    m:"ni" #optional
}

# classic boolean and
and {
    exp1:{}
    exp2:{}
    ...
}

# classic boolean or
or {
    exp1:{}
    exp2:{}
    ...
}

# is the string s behind string a and ahead of string b
look {
    a:["val"] # optional
    s:["val"] #required
    b:["val"] # optional
}

# the number of times a string may be contained in test string.
count { 
    s:["val"] #required

    # required.  as long as the number of instances of the val
    #  fall into the provided ranges, it returns true.
    qty:"1-5,7,8-10" #required
}
```

### Example

#### Test Data
```
blah
blah
foo
start
 this is a line
 this is another line
start
 this is foo
 this is more foo
bar
start
 this is foo
 this is more foo
```

#### Spec
```coffeescript
para {
    #optional include
    include: section {
        start: startswith {s:["foo"]}
        stop: startswith {s:["bar"]}
    }
    start: contains {s:["start"] m:"i"}
    props:[
        prop {
            name:"jason"
            line: contains {s:["is"] m:"i"}
            parsers:[
                after {
                    s:["is"]
                }
                to {
                    s:["line"]
                }

            ]
        }
        prop {
            name:"wall"
            line: contains {s:["an"] m:"i"}
            split:"foo" #optional
            parsers:[
                to {
                    s:["is"]
                }
            ]
        }
    ]
}

```