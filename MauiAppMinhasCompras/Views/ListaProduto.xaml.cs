using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{


    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();


    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = lista;
    }

    protected async override void OnAppearing()
    {

        try
        {
            var categorias = await App.Db.GetCategorias();
            pickerCategoria.ItemsSource = categorias;


            lista.Clear();
            List<Produto> tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.NovoProduto());

        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue;
            lst_produtos.IsRefreshing = true;
            lista.Clear();
            List<Produto> tmp = await App.Db.Search(q);
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }


    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            double soma = lista.Sum(i => i.Total);
            string msg = $"Valor total dos produtos: {soma:C}";
            DisplayAlert("Total dos Produtos", msg, "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem;
            Produto p = selecionado.BindingContext as Produto;
            bool confirm = await DisplayAlert("Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }


        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");


        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            if (e.SelectedItem != null)
            {
                Produto p = e.SelectedItem as Produto;
                Navigation.PushAsync(new Views.EditarProduto
                {
                    BindingContext = p,
                });
            }

        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            lista.Clear();
            List<Produto> tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    /*
    private async void ToolbarItem_Clicked_Relatorio(object sender, EventArgs e)
    {
        try
        {
            var produtos = await App.Db.GetAll();
            var relatorio = produtos
                .GroupBy(p => p.Categoria)
                .Select(g => new { Categoria = g.Key, Total = g.Sum(p => p.Total) })
                .ToList();

            string msg = string.Join("\n", relatorio.Select(r => $"{r.Categoria}: {r.Total:C}"));
            await DisplayAlert("Relatório de Gastos por Categoria", msg, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
    */

    private async void ToolbarItem_Clicked_Relatorio(object sender, EventArgs e)
    {
        try
        {
           
            var relatorio = await App.Db.GetRelatorioPorCategoria();

          
            string msg = "";
            foreach (var item in relatorio)
            {
                msg += $"{item.Categoria}: {item.Total:C}\n";
            }

            await DisplayAlert("Relatório de Gastos por Categoria", msg, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void FiltroPorCategoria(string categoria)
    {
        try
        {
            lista.Clear();
            List<Produto> tmp = await App.Db.SearchByCategoria(categoria);
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void pickerCategoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        string categoriaSelecionada = pickerCategoria.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(categoriaSelecionada))
        {
            FiltroPorCategoria(categoriaSelecionada);
        }
    }





}