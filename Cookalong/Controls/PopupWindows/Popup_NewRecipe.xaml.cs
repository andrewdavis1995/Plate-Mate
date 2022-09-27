using Andrew_2_0_Libraries.Controllers;
using Andrew_2_0_Libraries.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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
        readonly Action<Recipe> _displayRecipeCallback;
        readonly RecipeController _controller;
        Popup_Confirmation? _confirmationPopup;
        Popup_Method _methodPopup;
        Popup_Ingredient _ingredientPopup;
        Popup_Timing _timingPopup;
        ImageSource _defaultPreview;
        string _imagePath;

        public static Popup_NewRecipe Instance;
        private uint _duration;
        const int MIN_RECIPE_NAME = 5;
        const int MIN_STEPS = 2;
        const int MIN_INGREDIENTS = 2;

        public Popup_NewRecipe(Grid parent, Recipe? recipe, RecipeController controller, Action<Recipe> displayRecipeCallback)
        {
            Instance = this;

            InitializeComponent();
            _defaultPreview = imgPreview.Source;
            _parent = parent;
            _saved = recipe?.GetRecipeId();
            _displayRecipeCallback = displayRecipeCallback;
            _controller = controller;
            inputServingSize.Initialise(false, 20, 2);

            cmdConfirm.Configure("Confirm", false);
            cmdDelete.Configure("Delete", true, "Cancel");
            cmdClose.Configure("Cancel", true, "Cancel");
            cmdNewIngredient.Configure("+");
            cmdNewMethodStep.Configure("+");
            cmdSelectImage.Configure("Select Image");

            inpVegetarian.AddTitle("Vegetarian");
            inpVegan.AddTitle("Vegan");
            inpDairy.AddTitle("Dairy Free");
            inpGluten.AddTitle("Gluten Free");

            imgTimeIcon.SetImage("Timing");
            cmdSetTime.Configure("Set Time");

            SetPreviewImage_();

            if (recipe != null)
                SetExistingDetails_(recipe);

            cmdDelete.Visibility = recipe == null ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SetExistingDetails_(Recipe recipe)
        {
            txtRecipeName.Text = recipe.GetRecipeName();

            _duration = recipe.GetSetTime();

            inpVegetarian.UpdateDisplay(recipe.IsVegetarian());
            inpVegan.UpdateDisplay(recipe.IsVegan());
            inpDairy.UpdateDisplay(recipe.IsDairyFree());
            inpGluten.UpdateDisplay(recipe.IsGlutenFree());

            inputServingSize.SetInputValue(recipe.GetServingSize());
            _imagePath = recipe.GetImagePath();

            SetPreviewImage_();

            foreach (var step in recipe.GetMethodSteps())
            {
                methodList.AddItem(step);
            }

            foreach (var ing in recipe.GetIngredients())
            {
                var ingDisp = new IngredientsDisplay(ing);
                ingDisp.MouseLeftButtonDown += IngDisp_MouseLeftButtonDown;
                stckIngredients.Children.Add(ingDisp);
            }

            SetTime_(_duration);
        }

        private void IngDisp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as IngredientsDisplay;
            EditIngredient(control);
        }

        private void CmdClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _confirmationPopup = new Popup_Confirmation("Confirm cancel", "Are you sure you want to go back? All unsaved changes will be lost.", () =>
            {
                _parent?.Children.Remove(_confirmationPopup);
            },
            () =>
            {
                _parent?.Children.Remove(_confirmationPopup);
                _parent?.Children.Remove(this);
            });

            PopupController.AboveAll(_confirmationPopup);
            _parent?.Children.Add(_confirmationPopup);
        }

        private void CmdDelete_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_saved == null) return;

            _confirmationPopup = new Popup_Confirmation("Confirm deletion", "You are about to delete this recipe. Are you sure you wish to do this? This action cannot be undone.", () =>
            {
                _parent?.Children.Remove(_confirmationPopup);
            },
            () =>
            {
                _controller.DeleteRecipe((Guid)_saved);
                _displayRecipeCallback?.Invoke(null);
                _parent?.Children.Remove(_confirmationPopup);
                _parent?.Children.Remove(this);
            });

            PopupController.AboveAll(_confirmationPopup);
            _parent?.Children.Add(_confirmationPopup);
        }

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
                    catch (Exception) { };

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
                            Recipe? newlyCreated = null;

                            byte dietary = 0;
                            if (inpVegetarian.IsChecked()) dietary += Recipe.FLAG_VEGGIE;
                            if (inpVegan.IsChecked()) dietary += Recipe.FLAG_VEGAN;
                            if (inpDairy.IsChecked()) dietary += Recipe.FLAG_DAIRY;
                            if (inpGluten.IsChecked()) dietary += Recipe.FLAG_GLUTEN;

                            if (_saved != null)
                                newlyCreated = _controller.UpdateRecipe((Guid)_saved, txtRecipeName.Text, ingredientList, GetMethodList_(), dietary, servings, _imagePath, _duration);
                            else
                                newlyCreated = _controller.AddRecipe(txtRecipeName.Text, ingredientList, GetMethodList_(), dietary, servings, _imagePath, _duration);

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

            if (!string.IsNullOrEmpty(error))
            {
                RecipeMenu.Instance.ShowError(error);
            }
        }

        private List<string> GetMethodList_()
        {
            var list = new List<string>();
            foreach (DraggableObject item in methodList.stckData.Children)
            {
                // ignore blocker
                if (item.Visibility == Visibility.Visible)
                {
                    list.Add(item.txtData.Text);
                }
            }
            return list;
        }

        private void CmdNewIngredient_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EditIngredient(null);
        }

        private void cmdSelectImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png";
            if (sfd.ShowDialog() == true)
            {
                _imagePath = sfd.FileName;
                SetPreviewImage_();
            }
        }

        private void SetPreviewImage_()
        {
            if (File.Exists(_imagePath))
            {
                imgPreview.Source = new BitmapImage(new Uri(_imagePath, UriKind.Absolute));
            }
            else
            {
                imgPreview.Source = _defaultPreview;
            }

            lblPhotoRemove.Visibility = string.IsNullOrEmpty(_imagePath) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void lblPhotoRemove_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _imagePath = string.Empty;
            SetPreviewImage_();
        }

        private void cmdNewMethodStep_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EditStep(null);
        }

        internal void EditStep(DraggableObject draggableObject)
        {
            _methodPopup = new Popup_Method(draggableObject, () =>
            {
                _parent?.Children.Remove(_methodPopup);
            },
            (a) =>
            {
                if (draggableObject != null)
                    draggableObject.txtData.Text = a;
                else
                    methodList.AddItem(a);

                _parent?.Children.Remove(_methodPopup);
            });

            PopupController.AboveAll(_methodPopup);
            _parent?.Children.Add(_methodPopup);
        }

        internal void EditIngredient(IngredientsDisplay clicked)
        {
            _ingredientPopup = new Popup_Ingredient(clicked?.GetIngredient(), () =>
            {
                _parent?.Children.Remove(_ingredientPopup);
            },
            (a) =>
            {
                if (clicked != null)
                {
                    clicked.SetContent(a);
                }
                else
                {
                    var ingDisp = new IngredientsDisplay(a);
                    ingDisp.MouseLeftButtonDown += IngDisp_MouseLeftButtonDown;
                    stckIngredients.Children.Add(ingDisp);
                }

                _parent?.Children.Remove(_ingredientPopup);
            });

            PopupController.AboveAll(_ingredientPopup);
            _parent?.Children.Add(_ingredientPopup);
        }

        private void SetTime_(uint v)
        {
            if (v == 0)
            {
                lblTimeOutput.Content = "No time specified";
            }
            else
            {
                var hours = v / 60;
                var minutes = v - (hours * 60);

                string str = "";
                if (hours > 0) str += hours + " hours";
                if (minutes > 0)
                {
                    if (hours > 0) str += hours + ", ";
                    str += minutes + " minutes";
                }

                lblTimeOutput.Content = str;
            }
        }

        private void cmdSetTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _timingPopup = new Popup_Timing(_duration, () =>
            {
                _parent?.Children.Remove(_timingPopup);
            },
            (a) =>
            {
                _duration = a;
                SetTime_(a);
                _parent?.Children.Remove(_timingPopup);
            });

            PopupController.AboveAll(_timingPopup);
            _parent?.Children.Add(_timingPopup);
        }
    }
}
