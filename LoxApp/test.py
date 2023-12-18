from termcolor import colored
# print(colored("User named '{}' already exists.".format(user.name),"red"))


class Tester:
    def __init__(self):
        self.testCount = 10
        self.passed = True
        self.data = "none" #file
        self.answerKey = ["RECURSION_TEST",
                        "1",
                        "13",
                        "55",
                        "233",
                        "CLOSURE_TESTS",
                        "global",
                        "global",
                        "CLASS_TESTS",
                        "Bagel",
                        "instance",
                        "DevonshireCream",
                        "Crunch_crunch_crunch!",
                        "Foo"]
        self.results = []
    
    def find_answer(self):
        
        for answer in self.answerKey:
            try:
                self.results.index(answer)
                print(colored("PASS  =   {}".format(answer), "green"))
            except ValueError:
                self.passed = False
                print(colored("FAIL  =   {}".format(answer),"red"))


    def run_tests(self, fileName):
        results = []
        with open(fileName,'rb') as lox:
            byte_info = lox.read()
            results = byte_info.decode('utf-16',errors='replace')
        
        self.results = results.split()
        
        self.find_answer()

        if(self.passed):
            print(colored("ALL TESTS PASSED","green"))
        else:
            print(colored("TEST FAILED","red"))


if __name__ == "__main__":
    
    loxOut = "output.txt"
    results = Tester()

    results.run_tests(loxOut)