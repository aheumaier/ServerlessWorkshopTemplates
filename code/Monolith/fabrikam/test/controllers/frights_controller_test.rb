require 'test_helper'

class FrightsControllerTest < ActionDispatch::IntegrationTest
  setup do
    @fright = frights(:one)
  end

  test "should get index" do
    get frights_url
    assert_response :success
  end

  test "should get new" do
    get new_fright_url
    assert_response :success
  end

  test "should create fright" do
    assert_difference('Fright.count') do
      post frights_url, params: { fright: { name: @fright.name } }
    end

    assert_redirected_to fright_url(Fright.last)
  end

  test "should show fright" do
    get fright_url(@fright)
    assert_response :success
  end

  test "should get edit" do
    get edit_fright_url(@fright)
    assert_response :success
  end

  test "should update fright" do
    patch fright_url(@fright), params: { fright: { name: @fright.name } }
    assert_redirected_to fright_url(@fright)
  end

  test "should destroy fright" do
    assert_difference('Fright.count', -1) do
      delete fright_url(@fright)
    end

    assert_redirected_to frights_url
  end
end
