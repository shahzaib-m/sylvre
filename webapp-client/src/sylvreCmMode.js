const mode = {
	start: [
    {regex: /"(?:[^\\]|\\.)*?(?:"|$)/, token: "string"},
		{regex: /(Sylvre)/, token: "special"},
    {regex: /(function)(\s+)([A-Za-z][A-Za-z0-9_]*)/,
		 token: ["keyword", null, "variable-2"]},
		{regex: /([A-Za-z][A-Za-z0-9_]*)(\()/,
			token: ["variable-2", null]},
    {regex: /(?:function|create|call|exit|with|if|loopfor|loopwhile|elseif|else|increment|decrement|AND|OR|GTHAN|GEQUAL|LTHAN|LEQUAL|EQUALS|PARAMS)\b/,
		 token: "keyword"},
		{regex: /(#)/, token: "atom"},
    {regex: /TRUE|FALSE|NOT|NULL/, token: "keyword"},
    {regex: /0x[a-f\d]+|(?:\.\d+|\d+\.?\d*)(?:e[-+]?\d+)?/i,
     token: "number"},
    {regex: /\/\/.*/, token: "comment"},
    {regex: /\/(?:[^\\]|\\.)*?\//, token: "variable-3"},
    {regex: /\/\*/, token: "comment", next: "comment"},
    {regex: /[-+/*=]+/, token: "operator"},
    {regex: /[<[(]/, indent: true},
    {regex: /[>\])]/, dedent: true},
    {regex: /[A-Za-z][A-Za-z0-9_]*[\w$]*/, token: "variable"},
  ],
  comment: [
    {regex: /.*?\*\//, token: "comment", next: "start"},
    {regex: /.*/, token: "comment"}
  ],
  meta: {
    dontIndentStates: ["comment"],
		lineComment: "//",
		electricChars: ">)]"
  }
}

export { mode };