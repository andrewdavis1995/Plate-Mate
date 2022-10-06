using Andrew_2_0_Libraries.Controllers;
using Andrew_2_0_Libraries.Models;
using Cookalong.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cookalong.Controls.PopupWindows
{
    /// <summary>
    /// Interaction logic for Popup_NewRecipe.xaml
    /// </summary>
    public partial class Popup_NewRecipe : UserControl
    {
        readonly Grid? _parent;
        readonly Guid? _saved;
        readonly Action<Recipe?> _displayRecipeCallback;
        readonly Action<string> ? _errorCallback;
        readonly RecipeController ? _controller;

        Popup_Confirmation? _confirmationPopup;
        Popup_Method? _methodPopup;
        Popup_Ingredient? _ingredientPopup;
        Popup_Timing? _timingPopup;
        ImageSource? _defaultPreview;
        string _imagePath = "";

        // const values
        private uint _duration;
        const int MIN_RECIPE_NAME = 5;
        const int MIN_STEPS = 2;
        const int MIN_INGREDIENTS = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The parent (where to add controls to)</param>
        /// <param name="recipe">The recipe to edit (can be null)</param>
        /// <param name="controller">The recipe controller</param>
        /// <param name="displayRecipeCallback">Callback to call to display all recipes</param>
        public Popup_NewRecipe(Grid ? parent, Recipe? recipe, RecipeController ? controller, Action<Recipe?> displayRecipeCallback, Action<string> ? errorCallback)
        {
            InitializeComponent();

            methodList.Configure(this);

            // store data
            _defaultPreview = imgPreview.Source;
            _parent = parent;
            _saved = recipe?.GetRecipeId();
            _displayRecipeCallback = displayRecipeCallback;
            _controller = controller;
            _errorCallback = errorCallback;
            
            // initialise input for serving size
            inputServingSize.Initialise(false, 20, 2);

            // configure buttons
            cmdConfirm.Configure("Confirm", false);
            cmdDelete.Configure("Delete", true, "Cancel");
            cmdClose.Configure("Cancel", true, "Cancel");
            cmdNewIngredient.Configure("+");
            cmdNewMethodStep.Configure("+");
            cmdSelectImage.Configure("Select Image");
            cmdSetTime.Configure("Set Time");

            // configure checkboxes for nutrition info
            inpVegetarian.AddTitle("Vegetarian");
            inpVegan.AddTitle("Vegan");
            inpDairy.AddTitle("Dairy Free");
            inpGluten.AddTitle("Gluten Free");

            // set icons and images
            imgTimeIcon.SetImage("Timing");
            SetPreviewImage_();

            // display existing data if there is any
            if (recipe != null)
                SetExistingDetails_(recipe);

            // hide delete button if this is a new recipe (nothing to delete!)
            cmdDelete.Visibility = recipe == null ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Displays existing data
        /// </summary>
        /// <param name="recipe">The recipe to display</param>
        private void SetExistingDetails_(Recipe recipe)
        {
            // recipe name
            txtRecipeName.Text = recipe.GetRecipeName();

            // duration
            _duration = recipe.GetSetTime();
            lblTimeOutput.Content = StringHelper.GetTimeString(recipe.GetSetTime());

            // dietary info
            inpVegetarian.UpdateDisplay(recipe.IsVegetarian());
            inpVegan.UpdateDisplay(recipe.IsVegan());
            inpDairy.UpdateDisplay(recipe.IsDairyFree());
            inpGluten.UpdateDisplay(recipe.IsGlutenFree());

            // serving size
            inputServingSize.SetInputValue(recipe.GetServingSize());
            
            // image
            _imagePath = recipe.GetImagePath();
            SetPreviewImage_();

            // method steps
            foreach (var step in recipe.GetMethodSteps())
            {
                methodList.AddItem(step, grdOverall);
            }

            // ingredients
            foreach (var ing in recipe.GetIngredients())
            {
                NewIngredientDisplay_(ing);
            }
        }

        /// <summary>
        /// Event handler for clicking an ingredient display
        /// </summary>
        private void IngDisp_MouseLeftButtonDown(IngredientsDisplay ing)
        {
            EditIngredient(ing);
        }

        /// <summary>
        /// Event handler for the close button
        /// </summary>
        private void CmdClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // ask for confirmation
            _confirmationPopup = new Popup_Confirmation("Confirm cancel", "Are you sure you want to go back? All unsaved changes will be lost.", () =>
            {
                // cancel callback
                _parent?.Children.Remove(_confirmationPopup);
            },
            () =>
            {
                // confirm callback
                _parent?.Children.Remove(_confirmationPopup);
                _parent?.Children.Remove(this);
            });

            // ensure popup appears above other controls
            PopupController.AboveAll(_confirmationPopup);
            _parent?.Children.Add(_confirmationPopup);
        }

        /// <summary>
        /// Event handler for the delete button
        /// </summary>
        private void CmdDelete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_saved == null) return;

            // ask for confirmation
            _confirmationPopup = new Popup_Confirmation("Confirm deletion", "You are about to delete this recipe. Are you sure you wish to do this? This action cannot be undone.", () =>
            {
                // cancel callback
                _parent?.Children.Remove(_confirmationPopup);
            },
            () =>
            {
                // confirm callback
                // delete
                _controller?.DeleteRecipe((Guid)_saved);

                // update UI
                _displayRecipeCallback?.Invoke(null);
                _parent?.Children.Remove(_confirmationPopup);
                _parent?.Children.Remove(this);
            });

            PopupController.AboveAll(_confirmationPopup);
            _parent?.Children.Add(_confirmationPopup);
        }

        /// <summary>
        /// Event handler for the confirm button
        /// </summary>
        private void CmdConfirm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string error = string.Empty;

            // validate name
            if (txtRecipeName.Text.Length >= MIN_RECIPE_NAME)
            {
                // check there are enough steps
                if (methodList.NumElements() >= MIN_STEPS)
                {
                    // check the serving size is valid
                    int servings = 1;
                    try
                    {
                        servings = (int)inputServingSize.GetValue();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                    // check there are enough ingredients
                    var ingredientList = new List<Ingredient>();
                    if (stckIngredients.Children.Count >= MIN_INGREDIENTS)
                    {
                        // check each ingredient is valid
                        bool valid = true;
                        foreach (IngredientsDisplay ing in stckIngredients.Children)
                        {
                            ingredientList.Add(ing.GetIngredient());
                        }

                        // if all good, add/remove the recipe
                        if (valid)
                        {
                            Recipe? newlyCreated;

                            // calculate the BYTE value for the dietary info
                            byte dietary = 0;
                            if (inpVegetarian.IsChecked()) dietary += Recipe.FLAG_VEGGIE;
                            if (inpVegan.IsChecked()) dietary += Recipe.FLAG_VEGAN;
                            if (inpDairy.IsChecked()) dietary += Recipe.FLAG_DAIRY;
                            if (inpGluten.IsChecked()) dietary += Recipe.FLAG_GLUTEN;

                            // add a new recipe, or save the existing one
                            if (_saved != null)
                                newlyCreated = _controller?.UpdateRecipe((Guid)_saved, txtRecipeName.Text, ingredientList, GetMethodList_(), dietary, servings, _imagePath, _duration);
                            else
                                newlyCreated = _controller?.AddRecipe(txtRecipeName.Text, ingredientList, GetMethodList_(), dietary, servings, _imagePath, _duration);

                            // remove this page
                            _parent?.Children.Remove(this);
                            _displayRecipeCallback?.Invoke(newlyCreated);
                        }
                    }
                    else
                        error = $"Too few ingredients specified. You must include at least {MIN_INGREDIENTS}";
                }
                else
                    error = $"Too few steps specified. You must include at least {MIN_STEPS}";
            }
            else
                error = $"Name is not long enough. Must be at least {MIN_RECIPE_NAME} characters";

            // if there was an error, report it
            if (!string.IsNullOrEmpty(error))
            {
                _errorCallback?.Invoke(error);
            }
        }

        /// <summary>
        /// Gets a list of all methods sppecified
        /// </summary>
        /// <returns>List of all methods</returns>
        private List<string> GetMethodList_()
        {
            var list = new List<string>();

            // add each item
            foreach (DraggableObject item in methodList.stckData.Children)
            {
                // ignore blocker (used in dragging)
                if (item.Visibility == Visibility.Visible)
                {
                    list.Add(item.txtData.Text);
                }
            }
            return list;
        }

        /// <summary>
        /// Event handler for the (+) ingredient button
        /// </summary>
        private void CmdNewIngredient_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EditIngredient(null);
        }

        /// <summary>
        /// Event handler for the button to select an image
        /// </summary>
        private void cmdSelectImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // select which file to use
            OpenFileDialog sfd = new OpenFileDialog
            {
                Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png"
            };

            // if confirmed, show the image
            if (sfd.ShowDialog() == true)
            {
                _imagePath = sfd.FileName;
                SetPreviewImage_();
            }
        }

        /// <summary>
        /// Shows the selected image of the meal
        /// </summary>
        private void SetPreviewImage_()
        {
            // check file exists
            if (File.Exists(_imagePath))
            {
                imgPreview.Source = new BitmapImage(new Uri(_imagePath, UriKind.Absolute));
            }
            else
            {
                imgPreview.Source = _defaultPreview;
            }

            // if there is an image shown, report how to remove it
            lblPhotoRemove.Visibility = string.IsNullOrEmpty(_imagePath) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Event handler for right clicking the preview image
        /// </summary>
        private void lblPhotoRemove_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // remove image and update UI
            _imagePath = string.Empty;
            SetPreviewImage_();
        }

        /// <summary>
        /// Event handler for the (+) method step button
        /// </summary>
        private void cmdNewMethodStep_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EditStep(null);
        }

        /// <summary>
        /// When a step is clicked, and needs to be edited
        /// </summary>
        /// <param name="draggableObject">The object that was clicked</param>
        internal void EditStep(DraggableObject ? draggableObject)
        {
            var existing = draggableObject != null ? draggableObject.txtData.Text : string.Empty;

            _methodPopup = new Popup_Method(existing, () =>
            {
                // cancel callback
                _parent?.Children.Remove(_methodPopup);
            },
            (a) =>
            {
                // confirm callback
                // update existing, where possible
                if (draggableObject != null)
                    draggableObject.txtData.Text = a;
                else
                    methodList.AddItem(a, grdOverall);

                _parent?.Children.Remove(_methodPopup);
            }, _errorCallback);

            // ensure popup appears above others
            PopupController.AboveAll(_methodPopup);
            _parent?.Children.Add(_methodPopup);
        }

        /// <summary>
        /// Edits the clicked ingredient
        /// </summary>
        /// <param name="clicked">The ingredient to edit</param>
        internal void EditIngredient(IngredientsDisplay? clicked)
        {
            _ingredientPopup = new Popup_Ingredient(clicked?.GetIngredient(), () =>
            {
                // cancel callback
                _parent?.Children.Remove(_ingredientPopup);
            },
            (a) =>
            {
                // confirm callback
                if (clicked != null)
                {
                    // update existing ingredient
                    clicked.SetContent(a);
                }
                else
                {
                    // display new ingredient
                    NewIngredientDisplay_(a);
                }

                _parent?.Children.Remove(_ingredientPopup);
            }, _errorCallback);

            // ensure the control is above others
            PopupController.AboveAll(_ingredientPopup);
            _parent?.Children.Add(_ingredientPopup);
        }

        /// <summary>
        /// Creates a display for the specified ingredient
        /// </summary>
        /// <param name="i">Ingredient to display</param>
        void NewIngredientDisplay_(Ingredient i)
        {
            var ingDisp = new IngredientsDisplay(i, stckIngredients, grdOverall);
            ingDisp.clickable.MouseLeftButtonDown += (o, s) => IngDisp_MouseLeftButtonDown(ingDisp);
            stckIngredients.Children.Add(ingDisp);
        }

        /// <summary>
        /// Event handler for the time button
        /// </summary>
        private void cmdSetTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _timingPopup = new Popup_Timing(_duration, () =>
            {
                // cancel callback
                _parent?.Children.Remove(_timingPopup);
            },
            (a) =>
            {
                // confirm callback
                _duration = a;
                lblTimeOutput.Content = StringHelper.GetTimeString(a);
                _parent?.Children.Remove(_timingPopup);
            });

            // ensure the control appears above all others
            PopupController.AboveAll(_timingPopup);
            _parent?.Children.Add(_timingPopup);
        }

        /// <summary>
        /// Event handler for when a key is pressed on the name input
        /// </summary>
        private void txtName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                // check if a space is allowed
                e.Handled = !StringHelper.IsSpaceAllowed(txtRecipeName.Text);
            }
        }

        /// <summary>
        /// Event handler for when the text changes on the name input
        /// </summary>
        private void txtName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // don't allow forbidden characters
            e.Handled = StringHelper.ForbiddenCharacter(e.Text);
        }
    }
}
