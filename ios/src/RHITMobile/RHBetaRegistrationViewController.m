//
//  RHBetaRegistrationViewController.m
//  RHIT Mobile Campus Directory
//
//  Copyright 2011 Rose-Hulman Institute of Technology
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

#import "RHBetaRegistrationViewController.h"
#import "RHBetaViewController.h"
#import "RHBeta.h"

#ifdef RHITMobile_RHBeta

#define kOFFSET_FOR_KEYBOARD 215.0


@implementation RHBetaRegistrationViewController

@synthesize nameField;
@synthesize emailField;
@synthesize betaViewController;
@synthesize operations;

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        self.operations = [[NSOperationQueue alloc] init];
    }
    return self;
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
}

- (void)viewDidUnload
{
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark - UITextFieldDelegate Methods

-(void)textFieldDidBeginEditing:(UITextField *)sender
{
    if ([sender isEqual:self.nameField])
    {
        //move the main view, so that the keyboard does not hide it.
        if  (self.view.frame.origin.y >= 0)
        {
            [self setViewMovedUp:YES];
        }
    }
}

- (void)setViewMovedUp:(BOOL)movedUp {
    [UIView beginAnimations:nil context:NULL];
    [UIView setAnimationDuration:0.25]; // if you want to slide up the view
    
    CGRect rect = self.view.frame;
    if (movedUp) {
        rect.origin.y -= kOFFSET_FOR_KEYBOARD;
        rect.size.height += kOFFSET_FOR_KEYBOARD;
    } else {
        rect.origin.y += kOFFSET_FOR_KEYBOARD;
        rect.size.height -= kOFFSET_FOR_KEYBOARD;
    }
    self.view.frame = rect;
    
    [UIView commitAnimations];
}


- (void)keyboardWillShow:(NSNotification *)notif
{
    //keyboard will be shown now. depending for which textfield is active, move up or move down the view appropriately
    
    if ([self.nameField isFirstResponder] && self.view.frame.origin.y >= 0)
    {
        [self setViewMovedUp:YES];
    }
    else if (![self.nameField isFirstResponder] && self.view.frame.origin.y < 0)
    {
        [self setViewMovedUp:NO];
    }
}


- (void)viewWillAppear:(BOOL)animated
{
    // register for keyboard notifications
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(keyboardWillShow:) 
                                                 name:UIKeyboardWillShowNotification object:self.view.window]; 
}

- (void)viewWillDisappear:(BOOL)animated
{
    // unregister for keyboard notifications while not visible.
    [[NSNotificationCenter defaultCenter] removeObserver:self name:UIKeyboardWillShowNotification object:nil]; 
}

- (IBAction)register:(id)sender {
    if (self.emailField.text.length < 1) {
        [[[UIAlertView alloc] initWithTitle:@"Email Address Required" message:@"You must enter an email address to register for the beta program" delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil] show];
    } else if (self.nameField.text.length < 1) {
        [[[UIAlertView alloc] initWithTitle:@"Name Required" message:@"You must enter your name to register for the beta program" delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil] show];
    } else {
        self.betaViewController.registrationName = self.nameField.text;
        self.betaViewController.registrationEmail = self.emailField.text;
        
        NSInvocationOperation* operation = [NSInvocationOperation alloc];
        operation = [operation
                      initWithTarget:self.betaViewController
                      selector:@selector(performRegistration)
                      object:nil];
        [self.operations addOperation:operation];
        
        [self dismissModalViewControllerAnimated:YES];
    }
}


@end

#endif
